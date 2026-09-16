using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace KASPAR.Converters;

/// <summary>Converts a card's IsSelected bool into a border/background brush for its card.</summary>
public sealed class SelectedBrushConverter : IValueConverter
{
    public Color SelectedColor { get; set; }
    public Color UnselectedColor { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language) =>
        new SolidColorBrush(value is true ? SelectedColor : UnselectedColor);

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
