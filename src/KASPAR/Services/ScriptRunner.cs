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

    /// <summary>Runs an in-process tweak on a worker thread and reports the outcome.</summary>
    public static Task<ScriptResult> RunTweakAsync(Action tweak) => Task.Run(() =>
    {
        try
        {
            tweak();
            return new ScriptResult(true, "Completed.");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return new ScriptResult(false, "Access denied - run KASPAR as administrator.");
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    });

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
}
