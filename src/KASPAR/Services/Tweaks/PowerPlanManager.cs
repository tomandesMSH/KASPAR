using System.Runtime.InteropServices;
using System.Text;

namespace KASPAR.Services.Tweaks;

internal enum BasePowerPlan
{
    PowerSaver,
    Balanced,
    HighPerformance,
    UltimatePerformance,
}

internal readonly record struct PowerPlanInfo(Guid Guid, string Name, bool IsActive);

/// <summary>
/// Manages Windows power plans via powrprof.dll (replaces PowerPlan.exe, which shelled out to powercfg.exe).
/// </summary>
internal static class PowerPlanManager
{
    private const uint AccessScheme = 16;
    private const uint ErrorNoMoreItems = 259;

    private static readonly Dictionary<BasePowerPlan, Guid> BaseSchemes = new()
    {
        [BasePowerPlan.PowerSaver] = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a"),
        [BasePowerPlan.Balanced] = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e"),
        [BasePowerPlan.HighPerformance] = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"),
        [BasePowerPlan.UltimatePerformance] = new Guid("e9a42b02-d5df-448d-aa00-03f14749eb61"),
    };

    public static IReadOnlyList<PowerPlanInfo> List()
    {
        var active = GetActiveScheme();
        var plans = new List<PowerPlanInfo>();
        var buffer = new byte[16];
        uint index = 0;

        while (true)
        {
            var bufferSize = (uint)buffer.Length;
            var status = PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, AccessScheme, index, buffer, ref bufferSize);
            if (status == ErrorNoMoreItems)
            {
                break;
            }
            if (status != 0)
            {
                throw new InvalidOperationException($"Could not enumerate power plans (error {status}).");
            }

            var guid = new Guid(buffer);
            plans.Add(new PowerPlanInfo(guid, ReadFriendlyName(guid), guid == active));
            index++;
        }

        return plans;
    }

    public static Guid GetActiveScheme()
    {
        var status = PowerGetActiveScheme(IntPtr.Zero, out var ptr);
        if (status != 0)
        {
            throw new InvalidOperationException($"Could not read the active power plan (error {status}).");
        }
        try
        {
            return Marshal.PtrToStructure<Guid>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>Duplicates a base scheme, renames the copy, and makes it active.</summary>
    public static Guid CreateFromBase(BasePowerPlan basePlan, string name)
    {
        using var basePtr = new NativeGuid(BaseSchemes[basePlan]);
        var dupStatus = PowerDuplicateScheme(IntPtr.Zero, basePtr.Pointer, out var newPtr);
        if (dupStatus != 0)
        {
            throw new InvalidOperationException($"Could not create the power plan (error {dupStatus}).");
        }

        Guid newGuid;
        try
        {
            newGuid = Marshal.PtrToStructure<Guid>(newPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(newPtr);
        }

        using var newPlanPtr = new NativeGuid(newGuid);

        var nameBytes = Encoding.Unicode.GetBytes(name + "\0");
        var renameStatus = PowerWriteFriendlyName(IntPtr.Zero, newPlanPtr.Pointer, IntPtr.Zero, IntPtr.Zero, nameBytes, (uint)nameBytes.Length);
        if (renameStatus != 0)
        {
            throw new InvalidOperationException($"The power plan was created, but could not be renamed (error {renameStatus}).");
        }

        var activateStatus = PowerSetActiveScheme(IntPtr.Zero, newPlanPtr.Pointer);
        if (activateStatus != 0)
        {
            throw new InvalidOperationException($"The power plan was created, but could not be activated (error {activateStatus}).");
        }

        return newGuid;
    }

    /// <summary>Deletes a power plan. Windows refuses to delete the currently active plan.</summary>
    public static void Delete(Guid scheme)
    {
        using var schemePtr = new NativeGuid(scheme);
        var status = PowerDeleteScheme(IntPtr.Zero, schemePtr.Pointer);
        if (status != 0)
        {
            throw new InvalidOperationException(status == 5
                ? "Access denied - run KASPAR as administrator."
                : $"Could not delete the power plan (error {status}). It may be the active plan - switch to another plan first.");
        }
    }

    private static string ReadFriendlyName(Guid scheme)
    {
        using var schemePtr = new NativeGuid(scheme);
        uint size = 0;
        PowerReadFriendlyName(IntPtr.Zero, schemePtr.Pointer, IntPtr.Zero, IntPtr.Zero, null, ref size);
        if (size == 0)
        {
            return scheme.ToString();
        }

        var buffer = new byte[size];
        var status = PowerReadFriendlyName(IntPtr.Zero, schemePtr.Pointer, IntPtr.Zero, IntPtr.Zero, buffer, ref size);
        if (status != 0)
        {
            return scheme.ToString();
        }

        return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
    }

    /// <summary>Unmanaged copy of a <see cref="Guid"/>, for the raw GUID* parameters powrprof.dll expects.</summary>
    private readonly struct NativeGuid : IDisposable
    {
        public IntPtr Pointer { get; }

        public NativeGuid(Guid guid)
        {
            Pointer = Marshal.AllocHGlobal(16);
            Marshal.Copy(guid.ToByteArray(), 0, Pointer, 16);
        }

        public void Dispose() => Marshal.FreeHGlobal(Pointer);
    }

    [DllImport("powrprof.dll")]
    private static extern uint PowerEnumerate(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingsGuid, uint AccessFlags, uint Index, byte[] Buffer, ref uint BufferSize);

    [DllImport("powrprof.dll")]
    private static extern uint PowerReadFriendlyName(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingsGuid, IntPtr PowerSettingGuid, byte[]? Buffer, ref uint BufferSize);

    [DllImport("powrprof.dll")]
    private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);

    [DllImport("powrprof.dll")]
    private static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, IntPtr SchemeGuid);

    [DllImport("powrprof.dll")]
    private static extern uint PowerDuplicateScheme(IntPtr RootPowerKey, IntPtr SourceSchemeGuid, out IntPtr DestinationSchemeGuid);

    [DllImport("powrprof.dll")]
    private static extern uint PowerWriteFriendlyName(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingsGuid, IntPtr PowerSettingGuid, byte[] Buffer, uint BufferSize);

    [DllImport("powrprof.dll")]
    private static extern uint PowerDeleteScheme(IntPtr RootPowerKey, IntPtr SchemeGuid);
}
