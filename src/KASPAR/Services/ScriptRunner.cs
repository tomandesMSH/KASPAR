using KASPAR.Models;

namespace KASPAR.Services;

public static class ScriptRunner
{
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
}
