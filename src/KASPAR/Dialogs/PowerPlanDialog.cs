using KASPAR.Models;
using KASPAR.Services.Tweaks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KASPAR.Dialogs;

/// <summary>Replaces PowerPlan.exe's console menu with create/delete dialogs.</summary>
internal static class PowerPlanDialog
{
    public static async Task<ScriptResult?> ShowAsync(XamlRoot xamlRoot)
    {
        var action = await ShowActionPickerAsync(xamlRoot);
        if (action is null)
        {
            return null;
        }

        return action == Action.Create
            ? await ShowCreateDialogAsync(xamlRoot)
            : await ShowDeleteDialogAsync(xamlRoot);
    }

    private enum Action
    {
        Create,
        Delete,
    }

    private static async Task<Action?> ShowActionPickerAsync(XamlRoot xamlRoot)
    {
        var options = new RadioButtons
        {
            ItemsSource = new[] { "Create a new power plan", "Delete a power plan" },
            SelectedIndex = 0,
        };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Power Plan Manager",
            Content = new StackPanel { Width = 360, Children = { options } },
            PrimaryButtonText = "Continue",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        {
            return null;
        }

        return options.SelectedIndex == 0 ? Action.Create : Action.Delete;
    }

    private static async Task<ScriptResult?> ShowCreateDialogAsync(XamlRoot xamlRoot)
    {
        var baseBox = new ComboBox
        {
            Header = "Base plan",
            ItemsSource = new[] { "Power saving", "Balanced", "Maximum performance", "Ultra performance" },
            SelectedIndex = 1,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        var nameBox = new TextBox { Header = "Name for the new plan" };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Create a Power Plan",
            Content = new StackPanel { Width = 360, Spacing = 12, Children = { baseBox, nameBox } },
            PrimaryButtonText = "Create",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                args.Cancel = true;
                nameBox.Header = "Name for the new plan (required)";
            }
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        {
            return null;
        }

        var basePlan = baseBox.SelectedIndex switch
        {
            0 => BasePowerPlan.PowerSaver,
            1 => BasePowerPlan.Balanced,
            2 => BasePowerPlan.HighPerformance,
            _ => BasePowerPlan.UltimatePerformance,
        };

        try
        {
            PowerPlanManager.CreateFromBase(basePlan, nameBox.Text.Trim());
            return new ScriptResult(true, $"\"{nameBox.Text.Trim()}\" was created and set as the active plan.");
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }

    private static async Task<ScriptResult?> ShowDeleteDialogAsync(XamlRoot xamlRoot)
    {
        IReadOnlyList<PowerPlanInfo> plans;
        try
        {
            plans = PowerPlanManager.List();
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }

        var options = plans.Select(p => new PlanOption(p)).ToList();
        var planBox = new ComboBox
        {
            Header = "Plan to delete",
            ItemsSource = options,
            SelectedIndex = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Delete a Power Plan",
            Content = new StackPanel
            {
                Width = 360,
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "The active plan (marked below) cannot be deleted until you switch to another one.",
                        TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                        Opacity = 0.7,
                    },
                    planBox,
                },
            },
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        {
            return null;
        }

        var selected = options[planBox.SelectedIndex];

        try
        {
            PowerPlanManager.Delete(selected.Plan.Guid);
            return new ScriptResult(true, $"\"{selected.Plan.Name}\" was deleted.");
        }
        catch (Exception ex)
        {
            return new ScriptResult(false, ex.Message);
        }
    }

    private sealed record PlanOption(PowerPlanInfo Plan)
    {
        public override string ToString() => Plan.IsActive ? $"{Plan.Name} (active)" : Plan.Name;
    }
}
