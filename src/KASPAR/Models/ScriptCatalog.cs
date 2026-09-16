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
                    Exe = "apply/DisableAdvertisementID_Apply.exe",
                    RevertExe = "revert/DisableAdvertisementID_Revert.exe",
                    DocsAnchor = "disableadvertisementid",
                },
                new()
                {
                    Id = "startads",
                    Name = "Remove Start Menu Ads",
                    Description = "Remove sponsored app suggestions in Start menu",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/StartAds_Apply.exe",
                    RevertExe = "revert/StartAds_Revert.exe",
                    DocsAnchor = "startads",
                },
                new()
                {
                    Id = "disabledynsrch",
                    Name = "Disable Dynamic Search",
                    Description = "Disables News/Articles in Start and Search",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/DisableDynamicSearch_Apply.exe",
                    RevertExe = "revert/DisableDynamicSearch_Revert.exe",
                    DocsAnchor = "disabledynamicsearch",
                },
                new()
                {
                    Id = "localaccount",
                    Name = "Local Account Setup",
                    Description = "Helps you create a local account",
                    Tag = ScriptTag.Moderate,
                    Exe = "LocalAccount.exe",
                    RevertExe = null,
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
                    Exe = "apply/ClassicRClickMenu_Apply.exe",
                    RevertExe = "revert/ClassicRClickMenu_Revert.exe",
                    DocsAnchor = "classicrclickmenu",
                },
                new()
                {
                    Id = "detailedsd",
                    Name = "Detailed Shutdown",
                    Description = "Shows you whats happening on shutdown",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/DetailedShutdown_Apply.exe",
                    RevertExe = "revert/DetailedShutdown_Revert.exe",
                    DocsAnchor = "detailedshutdown",
                },
                new()
                {
                    Id = "endtask",
                    Name = "Enable End Task",
                    Description = "Adds End Task option to taskbar",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/EndTask_Apply.exe",
                    RevertExe = "revert/EndTask_Revert.exe",
                    DocsAnchor = "endtask",
                },
                new()
                {
                    Id = "dropfileremove",
                    Name = "Remove Drop File prompt",
                    Description = "Disable drag-and-drop window",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/DropFileRemove_Apply.exe",
                    RevertExe = "revert/DropFileRemove_Revert.exe",
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
                    RevertExe = null,
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
                    RevertExe = null,
                    Interactive = true,
                    DocsAnchor = "devicesetupregion",
                },
                new()
                {
                    Id = "localsearch",
                    Name = "Local Search Only",
                    Description = "Restrict Windows Search to local files only",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/LocalSearch_Apply.exe",
                    RevertExe = "revert/LocalSearch_Revert.exe",
                    DocsAnchor = "localsearch",
                },
                new()
                {
                    Id = "shutdownnoupdate",
                    Name = "Shutdown & Don't Update",
                    Description = "Control whether Windows forces updates on shutdown",
                    Tag = ScriptTag.Moderate,
                    Exe = "ShutdownAndDontUpdate.exe",
                    RevertExe = null,
                    Interactive = true,
                    DocsAnchor = "shutdownanddontupdate",
                },
                new()
                {
                    Id = "askcopilotdisable",
                    Name = "Remove ask Copilot",
                    Description = "Removes the \"Ask Copilot\" option from the context menu.",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/AskCopilotDisable_Apply.exe",
                    RevertExe = "revert/AskCopilotDisable_Revert.exe",
                },
                new()
                {
                    Id = "micvolume",
                    Name = "Max Microphone Volume",
                    Description = "Sets your microphone volume to 100% and unmutes it",
                    Tag = ScriptTag.Safe,
                    Exe = "apply/micvolume_Apply.exe",
                    RevertExe = "revert/micvolume_Revert.exe",
                },
            },
        },
    };
}
