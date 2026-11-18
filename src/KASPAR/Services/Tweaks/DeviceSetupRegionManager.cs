using Microsoft.Win32;

namespace KASPAR.Services.Tweaks;

/// <summary>
/// Changes the user region and (if elevated) the device setup region (replaces DeviceSetupRegion.exe,
/// which read scripts/WindowsCountryListFixed.xlsx at runtime and wrote these same values via winreg).
/// </summary>
internal static class DeviceSetupRegionManager
{
    private const string UserGeoPath = @"Control Panel\International\Geo";
    private const string DeviceRegionPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Control Panel\DeviceRegion";

    public static void Apply(CountryRegion country)
    {
        using (var key = Registry.CurrentUser.CreateSubKey(UserGeoPath, writable: true)
            ?? throw new InvalidOperationException($@"Could not open HKCU\{UserGeoPath}."))
        {
            // Windows stores both values as strings, even though Nation is numeric.
            key.SetValue("Name", country.Abbreviation, RegistryValueKind.String);
            key.SetValue("Nation", country.NationId.ToString(), RegistryValueKind.String);
        }

        try
        {
            using var deviceKey = Registry.LocalMachine.OpenSubKey(DeviceRegionPath, writable: true);
            deviceKey?.DeleteValue("DeviceRegion", throwOnMissingValue: false);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            // The user region above still applied; only the device setup region needs admin rights,
            // same caveat the old script printed.
        }
    }
}
