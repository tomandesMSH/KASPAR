using System.Diagnostics;
using System.Text;

namespace KASPAR.Services.Tweaks;

/// <summary>Creates a System Restore point (replaces RestorePoint_Apply.exe + restorepointhelper.ps1).</summary>
internal static class RestorePoint
{
    private const string Script = """
        $key = 'HKLM:\Software\Microsoft\Windows NT\CurrentVersion\SystemRestore'
        $failed = $false
        # Windows normally allows one restore point per 24h; lift the limit just for this call.
        Set-ItemProperty -Path $key -Name SystemRestorePointCreationFrequency -Value 0 -Type DWord -Force
        try {
            Checkpoint-Computer -Description 'KASPAR Restore Point' -RestorePointType MODIFY_SETTINGS -ErrorAction Stop
        } catch {
            [Console]::Error.WriteLine("Failed to create restore point: $_")
            $failed = $true
        } finally {
            Set-ItemProperty -Path $key -Name SystemRestorePointCreationFrequency -Value 1440 -Type DWord -Force
        }
        if ($failed) { exit 1 }
        """;

    public static void Create()
    {
        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(Script));

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo("powershell.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            },
        };
        process.StartInfo.ArgumentList.Add("-NoProfile");
        process.StartInfo.ArgumentList.Add("-NonInteractive");
        process.StartInfo.ArgumentList.Add("-EncodedCommand");
        process.StartInfo.ArgumentList.Add(encoded);

        process.Start();
        var stderr = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(60_000))
        {
            try { process.Kill(entireProcessTree: true); } catch { /* already gone */ }
            throw new TimeoutException("Restore point creation timed out.");
        }

        if (process.ExitCode != 0)
        {
            var message = stderr.GetAwaiter().GetResult().Trim();
            throw new InvalidOperationException(message.Length == 0 ? $"PowerShell exited with code {process.ExitCode}" : message);
        }
    }
}
