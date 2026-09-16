using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace KASPAR.Models;

public sealed class ScriptCardViewModel : INotifyPropertyChanged
{
    private bool _isSelected;

    public ScriptCardViewModel(ScriptDefinition definition)
    {
        Definition = definition;
    }

    public ScriptDefinition Definition { get; }

    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string TagLabel => Definition.Tag.ToString().ToLowerInvariant();

    public SolidColorBrush TagBrush => Definition.Tag switch
    {
        ScriptTag.Safe => new SolidColorBrush(Color.FromArgb(0xFF, 0x04, 0xA2, 0xCC)),
        ScriptTag.Moderate => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xC8, 0x00)),
        _ => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x4D, 0x6D)),
    };

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
