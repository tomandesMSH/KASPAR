using KASPAR.Dialogs;
using KASPAR.Services.Tweaks;

namespace KASPAR.Models;

/// <summary>
/// The tweak catalog, ported verbatim from the old web/index.html "categories" array.
/// </summary>
public static class ScriptCatalog
{
    public static IReadOnlyList<ScriptCategory> Categories { get; } = new List<ScriptCategory>
    {
        new()
        {
            Name = "Privacy & Ads",
            Scripts = new List<ScriptDefinition>
            {
                new()
                {
                    Id = "disableadid",
                    Name = "Disable Advertisement ID",
                    Description = "Stops Windows from using your ad targeting ID",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.AdvertisementId.Apply,
                    Revert = TweakLibrary.AdvertisementId.Revert,
                    DocsAnchor = "disableadvertisementid",
                },
                new()
                {
                    Id = "startads",
                    Name = "Remove Start Menu Ads",
                    Description = "Remove sponsored app suggestions in Start menu",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.StartAds.Apply,
                    Revert = TweakLibrary.StartAds.Revert,
                    DocsAnchor = "startads",
                },
                new()
                {
                    Id = "disabledynsrch",
                    Name = "Disable Dynamic Search",
                    Description = "Disables News/Articles in Start and Search",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.DynamicSearch.Apply,
                    Revert = TweakLibrary.DynamicSearch.Revert,
                    DocsAnchor = "disabledynamicsearch",
                },
                new()
                {
                    Id = "localaccount",
                    Name = "Local Account Setup",
                    Description = "Helps you create a local account",
                    Tag = ScriptTag.Moderate,
                    Dialog = LocalAccountDialog.ShowAsync,
                    Interactive = true,
                    DocsAnchor = "localaccount",
                },
            },
        },
        new()
        {
            Name = "UI & Shell Tweaks",
            Scripts = new List<ScriptDefinition>
            {
                new()
                {
                    Id = "classicroclick",
                    Name = "Classic Right-Click Menu",
                    Description = "Restores the full context menu (Win10)",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.ClassicRClickApply,
                    Revert = TweakLibrary.ClassicRClickRevert,
                    DocsAnchor = "classicrclickmenu",
                },
                new()
                {
                    Id = "detailedsd",
                    Name = "Detailed Shutdown",
                    Description = "Shows you whats happening on shutdown",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.DetailedShutdown.Apply,
                    Revert = TweakLibrary.DetailedShutdown.Revert,
                    DocsAnchor = "detailedshutdown",
                },
                new()
                {
                    Id = "endtask",
                    Name = "Enable End Task",
                    Description = "Adds End Task option to taskbar",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.EndTask.Apply,
                    Revert = TweakLibrary.EndTask.Revert,
                    DocsAnchor = "endtask",
                },
                new()
                {
                    Id = "dropfileremove",
                    Name = "Remove Drop File prompt",
                    Description = "Disable drag-and-drop window",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.DropFileRemove.Apply,
                    Revert = TweakLibrary.DropFileRemove.Revert,
                    DocsAnchor = "dropfileremove",
                },
            },
        },
        new()
        {
            Name = "System & Performance",
            Scripts = new List<ScriptDefinition>
            {
                new()
                {
                    Id = "powerplan",
                    Name = "Power Plan Manager",
                    Description = "Switch to high-performance or battery saver anytime",
                    Tag = ScriptTag.Safe,
                    Exe = "PowerPlan.exe",
                    Interactive = true,
                    DocsAnchor = "powerplan",
                },
                new()
                {
                    Id = "devsetupregion",
                    Name = "Device Setup Region",
                    Description = "Bypass region-locked Windows features",
                    Tag = ScriptTag.Moderate,
                    Exe = "DeviceSetupRegion.exe",
                    Interactive = true,
                    DocsAnchor = "devicesetupregion",
                },
                new()
                {
                    Id = "localsearch",
                    Name = "Local Search Only",
                    Description = "Restrict Windows Search to local files only",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.LocalSearch.Apply,
                    Revert = TweakLibrary.LocalSearch.Revert,
                    DocsAnchor = "localsearch",
                },
                new()
                {
                    Id = "shutdownnoupdate",
                    Name = "Shutdown & Don't Update",
                    Description = "Control whether Windows forces updates on shutdown",
                    Tag = ScriptTag.Moderate,
                    Dialog = ShutdownFlyoutDialog.ShowAsync,
                    Interactive = true,
                    DocsAnchor = "shutdownanddontupdate",
                },
                new()
                {
                    Id = "askcopilotdisable",
                    Name = "Remove ask Copilot",
                    Description = "Removes the \"Ask Copilot\" option from the context menu.",
                    Tag = ScriptTag.Safe,
                    Apply = TweakLibrary.AskCopilot.Apply,
                    Revert = TweakLibrary.AskCopilot.Revert,
                },
                new()
                {
                    Id = "micvolume",
                    Name = "Max Microphone Volume",
                    Description = "Sets your microphone volume to 100% and unmutes it",
                    Tag = ScriptTag.Safe,
                    Apply = () => MicrophoneVolume.Set(1.0f),
                    Revert = () => MicrophoneVolume.Set(0.0f),
                },
            },
        },
    };
}
