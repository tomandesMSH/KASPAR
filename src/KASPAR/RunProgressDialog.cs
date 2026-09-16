using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace KASPAR;

/// <summary>
/// A reusable run/revert progress dialog: a scrolling colored log plus a progress bar,
/// mirroring the old run-overlay modal in web/index.html.
/// </summary>
public sealed class RunProgressDialog
{
    public enum LineStyle
    {
        Running,
        Ok,
        Error,
        Done,
    }

    private readonly ContentDialog _dialog;
    private readonly StackPanel _logPanel;
    private readonly ScrollViewer _scrollViewer;
    private readonly ProgressBar _progressBar;

    public RunProgressDialog(XamlRoot xamlRoot, string title)
    {
        _logPanel = new StackPanel { Spacing = 4 };
        _scrollViewer = new ScrollViewer
        {
            Content = _logPanel,
            Height = 180,
            Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x0D, 0x0D, 0x0F)),
            Padding = new Thickness(14),
        };
        _progressBar = new ProgressBar { Minimum = 0, Maximum = 100, Value = 0, Margin = new Thickness(0, 14, 0, 0) };

        var content = new StackPanel { Width = 420, Children = { _scrollViewer, _progressBar } };

        _dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = title,
            Content = content,
        };
    }

    public IAsyncOperation<ContentDialogResult> ShowAsync() => _dialog.ShowAsync();

    public void AddLog(string text, LineStyle style)
    {
        var brush = style switch
        {
            LineStyle.Ok => new SolidColorBrush(Color.FromArgb(0xFF, 0x04, 0xA2, 0xCC)),
            LineStyle.Running => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xC8, 0x00)),
            LineStyle.Error => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x4D, 0x6D)),
            _ => new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE8, 0xF0)),
        };
        _logPanel.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 11,
            FontFamily = new FontFamily("Consolas"),
            Foreground = brush,
            TextWrapping = TextWrapping.Wrap,
        });
        _scrollViewer.UpdateLayout();
        _scrollViewer.ChangeView(null, _scrollViewer.ScrollableHeight, null);
    }

    public void SetProgress(int percent) => _progressBar.Value = percent;

    public void Finish() => _dialog.CloseButtonText = "Done - Close";
}
