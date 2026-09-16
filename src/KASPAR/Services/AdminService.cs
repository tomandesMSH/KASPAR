using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KASPAR.Services;

public static class AdminService
{
    public static bool IsElevated()
    {
        using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Attempts to relaunch the current executable elevated via a UAC prompt.
    /// Returns true if a new elevated process was started (the caller should exit),
    /// false if the user declined UAC or the relaunch could not be started (the
    /// caller should continue running non-elevated).
    /// </summary>
    public static bool RelaunchAsAdmin()
    {
        var exePath = Environment.ProcessPath;
        if (string.IsNullOrEmpty(exePath))
        {
            return false;
        }

        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                Verb = "runas",
            });
            return process is not null;
        }
        catch (Win32Exception)
        {
            // ERROR_CANCELLED: the user declined the UAC prompt.
            return false;
        }
    }
}
