using System.ComponentModel;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Mirel.Classes.Entries;

public class SettingEntry : ReactiveObject
{
    public SettingEntry()
    {
        PropertyChanged += OnPropertyChanged;
    }
    

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }
}