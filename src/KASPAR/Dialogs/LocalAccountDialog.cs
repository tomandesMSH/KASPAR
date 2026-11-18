using KASPAR.Models;
using KASPAR.Services.Tweaks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KASPAR.Dialogs;

/// <summary>Replaces LocalAccount.exe's console prompts with a form dialog.</summary>
internal static class LocalAccountDialog
{
    public static async Task<ScriptResult?> ShowAsync(XamlRoot xamlRoot)
    {
        var usernameBox = new TextBox { Header = "Username", PlaceholderText = "e.g. Tom" };
        var passwordBox = new PasswordBox { Header = "Password", PlaceholderText = "Must meet this PC's password policy" };
        var adminCheck = new CheckBox { Content = "Make this account an administrator" };

        var content = new StackPanel
        {
            Width = 360,
            Spacing = 12,
            Children = { usernameBox, passwordBox, adminCheck },
        };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Create Local Account",
            Content = content,
            PrimaryButtonText = "Create",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        // Keep the dialog open on invalid input instead of closing it with nothing to show for it.
        dialog.PrimaryButtonClick += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                args.Cancel = true;
                usernameBox.Header = "Username (required)";
            }
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        {
            return null;
        }

        var username = usernameBox.Text.Trim();
        var password = passwordBox.Password;
        var asAdmin = adminCheck.IsChecked == true;

        try
        {
            LocalAccountManager.Create(username, password, asAdmin);
            return new ScriptResult(true, asAdmin
                ? $"\"{username}\" was created as an administrator."
                : $"\"{username}\" was created as a standard user.");
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }
}
