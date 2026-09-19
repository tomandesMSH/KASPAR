using Microsoft.Win32;

namespace KASPAR.Services.Tweaks;

/// <summary>
/// One registry value a tweak owns. <see cref="RevertValue"/> of null means "delete the value on revert".
/// </summary>
internal sealed record RegValue(RegistryHive Hive, string Path, string Name, object Value, object? RevertValue)
{
    public static RegValue Dword(RegistryHive hive, string path, string name, int value, int? revert) =>
        new(hive, path, name, value, revert);

    public static RegValue Str(RegistryHive hive, string path, string name, string value, string? revert) =>
        new(hive, path, name, value, revert);
}

/// <summary>A tweak made of plain registry writes; replaces the old winreg-based Python scripts.</summary>
internal sealed class RegistryTweak(params RegValue[] values)
{
    public void Apply()
    {
        foreach (var v in values)
        {
            Write(v, v.Value);
        }
    }

    public void Revert()
    {
        foreach (var v in values)
        {
            if (v.RevertValue is null)
            {
                DeleteValue(v);
            }
            else
            {
                Write(v, v.RevertValue);
            }
        }
    }

    private static void Write(RegValue v, object value)
    {
        using var baseKey = RegistryKey.OpenBaseKey(v.Hive, RegistryView.Default);
        using var key = baseKey.CreateSubKey(v.Path, writable: true);
        key.SetValue(v.Name, value, value is int ? RegistryValueKind.DWord : RegistryValueKind.String);
    }

    private static void DeleteValue(RegValue v)
    {
        using var baseKey = RegistryKey.OpenBaseKey(v.Hive, RegistryView.Default);
        using var key = baseKey.OpenSubKey(v.Path, writable: true);
        key?.DeleteValue(v.Name, throwOnMissingValue: false);
    }
}
