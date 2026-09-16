using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Graphics;
using Windows.UI;
using WinRT.Interop;

namespace KASPAR;

public sealed partial class SplashWindow : Window
{
    private const int Width = 480;
    private const int Height = 320;

    public SplashWindow()
    {
        InitializeComponent();

        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        AppWindow.MoveAndResize(new RectInt32(0, 0, Width, Height));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.SetBorderAndTitleBar(hasBorder: false, hasTitleBar: false);
        }

        CenterOnScreen(windowId);
    }

    private void CenterOnScreen(Microsoft.UI.WindowId windowId)
    {
        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
        var workArea = displayArea.WorkArea;
        var x = workArea.X + (workArea.Width - Width) / 2;
        var y = workArea.Y + (workArea.Height - Height) / 2;
        AppWindow.Move(new PointInt32(x, y));
    }

    public void SetStatus(string text, string type = "")
    {
        StatusText.Text = text;
        StatusText.Foreground = type switch
        {
            "ok" => new SolidColorBrush(Colors.LimeGreen),
            "error" => new SolidColorBrush(Colors.OrangeRed),
            _ => new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0x5A, 0x6A)),
        };
    }

    public void SetProgress(int percent)
    {
        if (percent < 0)
        {
            Progress.IsIndeterminate = true;
        }
        else
        {
            Progress.IsIndeterminate = false;
            Progress.Value = percent;
        }
    }

    public void SetStep(int index)
    {
        Ellipse[] steps = [Step0, Step1, Step2, Step3];
        for (var i = 0; i < steps.Length; i++)
        {
            steps[i].Fill = new SolidColorBrush(
                i < index ? Color.FromArgb(0xFF, 0x0A, 0x5A, 0x70) :
                i == index ? Color.FromArgb(0xFF, 0x04, 0xA2, 0xCC) :
                Color.FromArgb(0xFF, 0x1C, 0x1C, 0x20));
        }
    }

    public void SetVersion(string version) => VersionText.Text = version;
}
