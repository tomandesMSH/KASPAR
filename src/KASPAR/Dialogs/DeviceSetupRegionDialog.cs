using KASPAR.Models;
using KASPAR.Services.Tweaks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KASPAR.Dialogs;

/// <summary>Replaces DeviceSetupRegion.exe's console menu with a searchable country picker.</summary>
internal static class DeviceSetupRegionDialog
{
    public static async Task<ScriptResult?> ShowAsync(XamlRoot xamlRoot)
    {
        CountryRegion? selected = null;

        var searchBox = new AutoSuggestBox
        {
            Header = "Country",
            PlaceholderText = "Start typing a country name...",
            TextMemberPath = nameof(CountryRegion.Name),
        };

        searchBox.TextChanged += (sender, args) =>
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            selected = null;
            var query = sender.Text.Trim();
            sender.ItemsSource = query.Length == 0
                ? Array.Empty<CountryRegion>()
                : CountryRegions.All.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).Take(15).ToList();
        };

        searchBox.SuggestionChosen += (sender, args) =>
        {
            if (args.SelectedItem is CountryRegion country)
            {
                selected = country;
            }
        };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Device Setup Region",
            Content = new StackPanel { Width = 360, Children = { searchBox } },
            PrimaryButtonText = "Apply",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            if (selected is null)
            {
                args.Cancel = true;
                searchBox.Header = "Country (pick one from the suggestions)";
            }
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary || selected is not CountryRegion country)
        {
            return null;
        }

        try
        {
            DeviceSetupRegionManager.Apply(country);
            return new ScriptResult(true, $"Region set to {country.Name}. Some apps may need a sign-out/restart to see it.");
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
