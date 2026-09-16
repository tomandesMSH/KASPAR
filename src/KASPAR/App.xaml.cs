using System.Diagnostics;
using System.Reflection;
using KASPAR.Services;
using Microsoft.UI.Xaml;
using Velopack;

namespace KASPAR;

public partial class App : Application
{
    private const int SplashMinMs = 1500;

    private Window? _splashWindow;
    private Window? _mainWindow;

    public App()
    {
        // Must run before anything else - handles Velopack's own install/update lifecycle hooks.
        VelopackApp.Build().Run();

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (!AdminService.IsElevated())
        {
            if (AdminService.RelaunchAsAdmin())
            {
                // The elevated instance takes over; this one just quits.
                Exit();
                return;
            }
            // UAC was declined or the relaunch failed - continue non-elevated.
            // MainPage shows the admin-warning banner once it loads.
        }

        var splash = new SplashWindow();
        _splashWindow = splash;
        splash.Activate();

        var main = new MainWindow();
        _mainWindow = main;

        _ = RunStartupSequenceAsync(splash, main);
    }

    private static async Task RunStartupSequenceAsync(SplashWindow splash, Window main)
    {
        splash.SetVersion($"v{GetVersion()}");

        var stopwatch = Stopwatch.StartNew();

        splash.SetStep(0);
        splash.SetStatus("Loading KASPAR...");
        splash.SetProgress(-1);
        await Task.Delay(400);

        splash.SetStep(1);
        splash.SetStatus("Preparing scripts...");
        splash.SetProgress(50);
        await Task.Delay(400);

        splash.SetStep(2);
        splash.SetStatus("Checking for updates...");
        splash.SetProgress(75);
        await Task.Delay(600);

        splash.SetStep(3);
        splash.SetStatus("Ready!", "ok");
        splash.SetProgress(100);

        var remaining = TimeSpan.FromMilliseconds(SplashMinMs) - stopwatch.Elapsed;
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining);
        }

        main.Activate();
        await Task.Delay(150);
        splash.Close();
    }

    private static string GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version is null ? "1.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
    }
}
