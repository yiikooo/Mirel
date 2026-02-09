using System.ComponentModel;
using Avalonia.Media;
using Mirel.Classes.Enums;
using Mirel.Module.App;
using Mirel.Module.Ui;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mirel.Classes.Entries;

public class SettingEntry : ReactiveObject
{
    public SettingEntry()
    {
        PropertyChanged += OnPropertyChanged;
    }

    [Reactive] [JsonProperty] public Setting.Theme Theme { get; set; } = Setting.Theme.Dark;

    [Reactive]
    [JsonProperty]
    public NavPageEntry LaunchPage { get; set; } = new()
    {
        Identifier = PageIdentifier.NewTab,
    };

    [Reactive] [JsonProperty] public Color ThemeColor { get; set; } = Color.Parse("#1BD76A");
    [Reactive] [JsonProperty] public Color BackGroundColor { get; set; } = Color.Parse("#00B7FF52");
    [Reactive] [JsonProperty] public bool UseFilePicker { get; set; } = true;
    [Reactive] [JsonProperty] public bool AutoCheckUpdate { get; set; } = true;
    [Reactive] [JsonProperty] public bool EnableGithubProxy { get; set; } = true;
    [Reactive] [JsonProperty] public bool EnableAside { get; set; }
    [Reactive] [JsonProperty] public string GithubProxy { get; set; } = "https://ghproxy.net/%url%";

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Theme))
            Setter.ToggleTheme(Theme);
        else if (e.PropertyName == nameof(ThemeColor)) Setter.SetAccentColor(ThemeColor);

        AppMethod.SaveSetting();
    }
}