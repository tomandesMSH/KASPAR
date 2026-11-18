using Microsoft.Win32;

namespace KASPAR.Services.Tweaks;

/// <summary>Which entries Windows offers in the Start menu power flyout.</summary>
internal enum ShutdownFlyoutMode
{
    /// <summary>Plain "Shut down"/"Restart" are back next to the update variants (ShutdownFlyoutOptions = 0xF).</summary>
    UpdatesOptional,
    /// <summary>Windows default: only "Update and shut down"/"Update and restart" (0xA).</summary>
    UpdatesForced,
    /// <summary>No update entries at all, just "Shut down"/"Restart" (0x0).</summary>
    UpdatesHidden,
}

/// <summary>
/// The power-flyout options tweak (replaces ShutdownAndDontUpdate.exe).
/// Windows reads this value from the 64-bit view only, hence <see cref="RegistryView.Registry64"/>.
/// </summary>
internal static class ShutdownFlyout
{
    private const string Path = @"SOFTWARE\Microsoft\WindowsUpdate\Orchestrator";
    private const string ValueName = "ShutdownFlyoutOptions";

    public static void Set(ShutdownFlyoutMode mode)
    {
        var value = mode switch
        {
            ShutdownFlyoutMode.UpdatesOptional => 0xF,
            ShutdownFlyoutMode.UpdatesForced => 0xA,
            ShutdownFlyoutMode.UpdatesHidden => 0x0,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };

        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using var key = baseKey.CreateSubKey(Path, writable: true)
            ?? throw new InvalidOperationException($@"Could not open HKLM\{Path}.");
        key.SetValue(ValueName, value, RegistryValueKind.DWord);
    }

    /// <summary>The mode currently configured, or null when Windows has not been told either way.</summary>
    public static ShutdownFlyoutMode? Current()
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(Path);
            return key?.GetValue(ValueName) switch
            {
                0xF => ShutdownFlyoutMode.UpdatesOptional,
                0xA => ShutdownFlyoutMode.UpdatesForced,
                0x0 => ShutdownFlyoutMode.UpdatesHidden,
                _ => null,
            };
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return null;
        }
    }
}
