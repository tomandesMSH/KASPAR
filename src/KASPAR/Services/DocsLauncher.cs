using System.Diagnostics;

namespace KASPAR.Services;

/// <summary>
/// Opens links in the system default browser. All doc/nav/about links use this -
/// no in-app webview window, per the decision to keep the app free of any
/// web-rendered UI.
/// </summary>
public static class DocsLauncher
{
    public static void OpenExternal(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch
        {
            // best effort - nothing sensible to do if the shell can't handle the URL.
        }
    }
}
