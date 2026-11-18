using KASPAR.Models;
using KASPAR.Services.Tweaks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KASPAR.Dialogs;

/// <summary>Replaces ShutdownAndDontUpdate.exe's console menu with a picker dialog.</summary>
internal static class ShutdownFlyoutDialog
{
    public static async Task<ScriptResult?> ShowAsync(XamlRoot xamlRoot)
    {
        var options = new RadioButtons
        {
            Header = "Start menu power button options",
            ItemsSource = new[]
            {
                "Restore both (Shutdown/Restart and the Update variants)",
                "Update variants only - Windows default",
                "Remove the Update option entirely",
            },
        };

        options.SelectedIndex = ShutdownFlyout.Current() switch
        {
            ShutdownFlyoutMode.UpdatesOptional => 0,
            ShutdownFlyoutMode.UpdatesForced => 1,
            ShutdownFlyoutMode.UpdatesHidden => 2,
            _ => 0,
        };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Shutdown & Don't Update",
            Content = new StackPanel { Width = 380, Children = { options } },
            PrimaryButtonText = "Apply",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        {
            return null;
        }

        var mode = options.SelectedIndex switch
        {
            0 => ShutdownFlyoutMode.UpdatesOptional,
            1 => ShutdownFlyoutMode.UpdatesForced,
            _ => ShutdownFlyoutMode.UpdatesHidden,
        };

        try
        {
            ShutdownFlyout.Set(mode);
            return new ScriptResult(true, "Power menu options updated.");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException)
        {
            return new ScriptResult(false, "Access denied - run KASPAR as administrator.");
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }
}
