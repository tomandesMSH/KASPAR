using System.Diagnostics;
using Microsoft.Win32;

namespace KASPAR.Services.Tweaks;

/// <summary>
/// The tweaks themselves - ported 1:1 from the former PyInstaller scripts in scripts/apply and scripts/revert.
/// KASPAR relaunches itself elevated at startup, so no per-tweak UAC handling is needed here.
/// </summary>
internal static class TweakLibrary
{
    private const RegistryHive HKCU = RegistryHive.CurrentUser;
    private const RegistryHive HKLM = RegistryHive.LocalMachine;

    private const string ContentDeliveryManager = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
    private const string ClassicMenuClsid = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";

    public static readonly RegistryTweak AdvertisementId = new(
        RegValue.Dword(HKCU, @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, 1),
        RegValue.Dword(HKCU, @"Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0, 1),
        RegValue.Dword(HKCU, ContentDeliveryManager, "SubscribedContent-338393Enabled", 0, 1),
        RegValue.Dword(HKCU, ContentDeliveryManager, "SubscribedContent-353694Enabled", 0, 1),
        RegValue.Dword(HKLM, @"Software\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, 3));

    public static readonly RegistryTweak StartAds = new(
        RegValue.Dword(HKCU, ContentDeliveryManager, "SystemPaneSuggestionsEnabled", 0, 1));

    public static readonly RegistryTweak DynamicSearch = new(
        RegValue.Dword(HKCU, @"Software\Microsoft\Windows\CurrentVersion\SearchSettings", "IsDynamicSearchBoxEnabled", 0, 1));

    public static readonly RegistryTweak DetailedShutdown = new(
        RegValue.Dword(HKLM, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "VerboseStatus", 1, null));

    public static readonly RegistryTweak EndTask = new(
        RegValue.Dword(HKCU, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings", "TaskbarEndTask", 1, null));

    public static readonly RegistryTweak DropFileRemove = new(
        RegValue.Dword(HKLM, @"SYSTEM\ControlSet001\Control\FeatureManagement\Overrides\14\3895955085", "EnabledState", 1, null),
        RegValue.Dword(HKLM, @"SYSTEM\ControlSet001\Control\FeatureManagement\Overrides\14\3895955085", "EnabledStateOptions", 0, null));

    public static readonly RegistryTweak LocalSearch = new(
        RegValue.Dword(HKCU, @"Software\Policies\Microsoft\Explorer", "DisableSearchBoxSuggestions", 1, null),
        RegValue.Dword(HKLM, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1, null));

    public static readonly RegistryTweak AskCopilot = new(
        RegValue.Str(HKCU, @"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked", "{CB3B0003-8088-4EDE-8769-8B354AB2FF8C}", "", null),
        RegValue.Str(HKCU, @"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked", "{241F3F77-3B9E-4C64-8E2E-1A7C62C2F1AE}", "", null));

    /// <summary>Empty InprocServer32 default value under this CLSID makes Explorer use the classic context menu.</summary>
    public static void ClassicRClickApply()
    {
        using (var key = Registry.CurrentUser.CreateSubKey($@"{ClassicMenuClsid}\InprocServer32", writable: true))
        {
            key.SetValue("", "", RegistryValueKind.String);
        }
        RestartExplorer();
    }

    public static void ClassicRClickRevert()
    {
        Registry.CurrentUser.DeleteSubKey($@"{ClassicMenuClsid}\InprocServer32", throwOnMissingSubKey: false);
        Registry.CurrentUser.DeleteSubKey(ClassicMenuClsid, throwOnMissingSubKey: false);
        RestartExplorer();
    }

    private static void RestartExplorer()
    {
        using (var kill = Process.Start(new ProcessStartInfo("taskkill.exe", "/im explorer.exe /f")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
        }))
        {
            if (kill is not null && !kill.WaitForExit(15_000))
            {
                throw new TimeoutException("Timed out while restarting Explorer.");
            }
        }

        using var _ = Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
    }
}
