using System.Diagnostics;
using KASPAR.Models;

namespace KASPAR.Services;

public static class ScriptRunner
{
    public static string ScriptsRoot()
    {
#if DEBUG
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "scripts")))
        {
            dir = dir.Parent;
        }
        if (dir is not null)
        {
            return Path.Combine(dir.FullName, "scripts");
        }
#endif
        return Path.Combine(AppContext.BaseDirectory, "scripts");
    }

    /// <summary>Runs a script silently and waits for it to finish, capturing its output.</summary>
    public static async Task<ScriptResult> RunScriptAsync(string exe)
    {
        var scriptPath = Path.Combine(ScriptsRoot(), exe);

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(scriptPath)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                },
            };
            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var stdout = (await stdoutTask).Trim();
            var stderr = (await stderrTask).Trim();

            if (process.ExitCode == 0)
            {
                return new ScriptResult(true, stdout.Length == 0 ? "Completed." : stdout);
            }

            return new ScriptResult(false, stderr.Length == 0 ? $"Exited with code {process.ExitCode}" : stderr);
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }

    /// <summary>Runs a script interactively - it opens its own console window and the caller does not wait.</summary>
    public static ScriptResult RunInteractive(string exe)
    {
        var scriptPath = Path.Combine(ScriptsRoot(), exe);

        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{scriptPath}\"",
                UseShellExecute = true,
            });
            return new ScriptResult(true, string.Empty);
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }

    /// <summary>
    /// Runs a script elevated (triggers a UAC prompt) and captures its output via a temp-file
    /// redirect, since a UAC-elevated child process cannot inherit stdio pipes across the
    /// elevation boundary.
    /// </summary>
    public static async Task<ScriptResult> RunElevatedAsync(string exe)
    {
        var scriptPath = Path.Combine(ScriptsRoot(), exe);
        var tmpOut = Path.Combine(Path.GetTempPath(), $"kaspar_out_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.txt");

        var psCommand =
            $"$p = Start-Process -FilePath {PsQuote(scriptPath)} -Verb RunAs -Wait -PassThru " +
            $"-RedirectStandardOutput {PsQuote(tmpOut)} -RedirectStandardError {PsQuote(tmpOut)}; exit $p.ExitCode";

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };
            process.StartInfo.ArgumentList.Add("-NoProfile");
            process.StartInfo.ArgumentList.Add("-NonInteractive");
            process.StartInfo.ArgumentList.Add("-Command");
            process.StartInfo.ArgumentList.Add(psCommand);

            process.Start();
            await process.WaitForExitAsync();

            var output = File.Exists(tmpOut) ? (await File.ReadAllTextAsync(tmpOut)).Trim() : string.Empty;
            try { File.Delete(tmpOut); } catch { /* best effort */ }

            return process.ExitCode switch
            {
                1223 => new ScriptResult(false, "Cancelled: UAC prompt was dismissed."),
                0 => new ScriptResult(true, output.Length == 0 ? "Completed." : output),
                var code => new ScriptResult(false, output.Length == 0 ? $"Exited with code {code}" : output),
            };
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }

    private static string PsQuote(string path) => "'" + path.Replace("'", "''") + "'";
}
