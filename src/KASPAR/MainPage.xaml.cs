using KASPAR.Models;
using KASPAR.Services;
using KASPAR.Services.Tweaks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KASPAR;

public sealed partial class MainPage : Page
{
    private const string DocsBase = "https://tomandes.eu/KASPARDocs/";

    private readonly List<CategoryViewModel> _categories;
    private readonly List<ScriptCardViewModel> _allCards;
    private readonly UpdateService _updateService = new();

    public MainPage()
    {
        InitializeComponent();

        _categories = ScriptCatalog.Categories
            .Select(c => new CategoryViewModel
            {
                Name = c.Name,
                Scripts = c.Scripts.Select(s => new ScriptCardViewModel(s)).ToList(),
            })
            .ToList();
        _allCards = _categories.SelectMany(c => c.Scripts).ToList();

        CategoriesList.ItemsSource = _categories;

        Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        AdminWarning.IsOpen = !AdminService.IsElevated();
        await CheckForUpdatesAsync();
    }

    // ─── Selection ──────────────────────────────────────────────────────────

    private void ScriptCard_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: ScriptCardViewModel card })
        {
            card.IsSelected = !card.IsSelected;
            UpdateSelectionUi();
        }
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var card in _allCards)
        {
            card.IsSelected = true;
        }
        UpdateSelectionUi();
    }

    private void SelectNone_Click(object sender, RoutedEventArgs e)
    {
        foreach (var card in _allCards)
        {
            card.IsSelected = false;
        }
        UpdateSelectionUi();
    }

    private void UpdateSelectionUi()
    {
        var selected = _allCards.Where(c => c.IsSelected).ToList();
        var count = selected.Count;

        SelectionCountText.Text = $"{count} selected";
        RunButton.Content = $"Run Tweaks ({count})";
        RunButton.IsEnabled = count > 0;

        var canRevert = count > 0 && selected.All(c => c.Definition.Revert is not null);
        RevertButton.Content = $"Revert ({count})";
        RevertButton.IsEnabled = canRevert;
        ToolTipService.SetToolTip(RevertButton, canRevert || count == 0
            ? null
            : "One or more selected tweaks cannot be reverted");
    }

    // ─── Run / Revert / Restore ─────────────────────────────────────────────

    private async void Run_Click(object sender, RoutedEventArgs e)
    {
        var toRun = _allCards.Where(c => c.IsSelected).Select(c => c.Definition).ToList();
        if (toRun.Count == 0)
        {
            return;
        }

        var interactive = toRun.Where(s => s.Interactive).ToList();
        var silent = toRun.Where(s => !s.Interactive).ToList();

        foreach (var s in interactive)
        {
            if (s.Dialog is not null)
            {
                var result = await s.Dialog(XamlRoot);
                if (result is not null)
                {
                    StatusMessageText.Text = result.Success
                        ? $"{s.Name}: {result.Message}"
                        : $"{s.Name} failed: {result.Message}";
                }
            }
            else
            {
                ScriptRunner.RunInteractive(s.Exe!);
            }
        }

        if (silent.Count == 0)
        {
            return;
        }

        var dialog = new RunProgressDialog(XamlRoot, "Running Scripts...");
        _ = dialog.ShowAsync();

        int succeeded = 0, failed = 0;
        for (var i = 0; i < silent.Count; i++)
        {
            var s = silent[i];
            dialog.AddLog($"Running: {s.Name}...", RunProgressDialog.LineStyle.Running);
            var result = await ScriptRunner.RunTweakAsync(s.Apply!);

            if (result.Success)
            {
                dialog.AddLog($"  {s.Name} - completed", RunProgressDialog.LineStyle.Ok);
                succeeded++;
            }
            else
            {
                dialog.AddLog($"  {s.Name} - failed: {result.Message}", RunProgressDialog.LineStyle.Error);
                failed++;
            }
            dialog.SetProgress((i + 1) * 100 / silent.Count);
        }

        var summary = failed == 0
            ? $"All {succeeded} script(s) completed successfully."
            : $"Done: {succeeded} succeeded, {failed} failed.";
        dialog.AddLog(summary, failed == 0 ? RunProgressDialog.LineStyle.Done : RunProgressDialog.LineStyle.Error);
        dialog.Finish();

        StatusMessageText.Text = $"Last run: {succeeded}/{silent.Count} scripts OK";
    }

    private async void Revert_Click(object sender, RoutedEventArgs e)
    {
        var toRevert = _allCards.Where(c => c.IsSelected && c.Definition.Revert is not null)
            .Select(c => c.Definition).ToList();
        if (toRevert.Count == 0)
        {
            return;
        }

        var dialog = new RunProgressDialog(XamlRoot, "Reverting Tweaks...");
        _ = dialog.ShowAsync();

        int succeeded = 0, failed = 0;
        for (var i = 0; i < toRevert.Count; i++)
        {
            var s = toRevert[i];
            dialog.AddLog($"Reverting: {s.Name}...", RunProgressDialog.LineStyle.Running);
            try
            {
                var runTask = ScriptRunner.RunTweakAsync(s.Revert!);
                var completed = await Task.WhenAny(runTask, Task.Delay(TimeSpan.FromSeconds(10)));

                if (completed != runTask)
                {
                    throw new TimeoutException("Timed out, tweak was probably applied/reverted but did not confirm it, this is a common issue and is being worked on.");
                }

                var result = await runTask;
                if (result.Success)
                {
                    dialog.AddLog($"  {s.Name} - reverted", RunProgressDialog.LineStyle.Ok);
                    succeeded++;
                }
                else
                {
                    dialog.AddLog($"  {s.Name} - failed: {result.Message}", RunProgressDialog.LineStyle.Error);
                    failed++;
                }
            }
            catch (Exception ex)
            {
                dialog.AddLog($"  {s.Name} - error: {ex.Message}", RunProgressDialog.LineStyle.Error);
                failed++;
            }
            dialog.SetProgress((i + 1) * 100 / toRevert.Count);
        }

        var summary = failed == 0
            ? $"All {succeeded} tweak(s) reverted successfully."
            : $"Done: {succeeded} reverted, {failed} failed.";
        dialog.AddLog(summary, failed == 0 ? RunProgressDialog.LineStyle.Done : RunProgressDialog.LineStyle.Error);
        dialog.Finish();

        StatusMessageText.Text = $"Last revert: {succeeded}/{toRevert.Count} scripts OK";
        UpdateSelectionUi();
    }

    private async void RestorePoint_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new RunProgressDialog(XamlRoot, "Creating Restore Point...");
        _ = dialog.ShowAsync();

        dialog.AddLog("Creating restore point...", RunProgressDialog.LineStyle.Running);
        var result = await ScriptRunner.RunTweakAsync(RestorePoint.Create);
        dialog.AddLog(
            result.Success ? "Restore point created successfully." : $"Failed: {result.Message}",
            result.Success ? RunProgressDialog.LineStyle.Ok : RunProgressDialog.LineStyle.Error);
        dialog.SetProgress(100);
        dialog.Finish();
    }

    // ─── Nav / docs links ───────────────────────────────────────────────────

    private void AboutNav_Click(object sender, RoutedEventArgs e) => DocsLauncher.OpenExternal($"{DocsBase}about.html");
    private void DocsNav_Click(object sender, RoutedEventArgs e) => DocsLauncher.OpenExternal($"{DocsBase}index.html");
    private void GitHubNav_Click(object sender, RoutedEventArgs e) => DocsLauncher.OpenExternal("https://github.com/tomandesmsh/KASPAR");
    private void BugNav_Click(object sender, RoutedEventArgs e) => DocsLauncher.OpenExternal($"{DocsBase}bug.html");
    private void SuggestionsNav_Click(object sender, RoutedEventArgs e) => DocsLauncher.OpenExternal("https://forms.gle/pxvZQtuw6dKZWEZv8");

    // ─── Updater ────────────────────────────────────────────────────────────

    private async Task CheckForUpdatesAsync()
    {
        var update = await _updateService.CheckForUpdatesAsync();
        if (update is null)
        {
            return;
        }

        var confirmDownload = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = $"KASPAR v{update.TargetFullRelease.Version} is available.",
            Content = "A new version was detected. Do you want to download and install it now?",
            PrimaryButtonText = "Download and Install",
            CloseButtonText = "Later",
            DefaultButton = ContentDialogButton.Primary,
        };
        if (await confirmDownload.ShowAsync() != ContentDialogResult.Primary)
        {
            return;
        }

        var progressBar = new ProgressBar { Minimum = 0, Maximum = 100, Value = 0 };
        var progressDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = "Downloading Update...",
            Content = progressBar,
        };
        var showTask = progressDialog.ShowAsync();

        await _updateService.DownloadUpdatesAsync(update, percent =>
            DispatcherQueue.TryEnqueue(() => progressBar.Value = percent));

        progressDialog.Hide();

        var confirmRestart = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = "Update downloaded.",
            Content = "KASPAR will restart and install the update now.",
            PrimaryButtonText = "Restart Now",
            DefaultButton = ContentDialogButton.Primary,
        };
        await confirmRestart.ShowAsync();

        _updateService.ApplyUpdatesAndRestart(update);
    }
}
