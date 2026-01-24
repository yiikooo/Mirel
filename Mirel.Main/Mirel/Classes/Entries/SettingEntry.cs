using System;
using System.ComponentModel;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using HarfBuzzSharp;
using Mirel.Views.Main.Pages;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Mirel.Classes.Entries;

public class SettingEntry : ReactiveObject
{
    public SettingEntry()
    {
        PropertyChanged += OnPropertyChanged;
    }

    [Reactive] [JsonProperty] public Enums.Setting.NoticeWay NoticeWay { get; set; } = Enums.Setting.NoticeWay.Bubble;
    [Reactive] [JsonProperty] public Enums.Setting.Theme Theme { get; set; } = Enums.Setting.Theme.Dark;

    [Reactive]
    [JsonProperty]
    public Enums.Setting.BackGround BackGround { get; set; } = Enums.Setting.BackGround.Default;

    [Reactive]
    [JsonProperty]
    public LaunchPageEntry LaunchPage { get; set; } = new()
    {
        Id = "NewTab",
        Header = "新标签页",
        Page = new NewTabPage()
    };

    [Reactive] [JsonProperty] public Color ThemeColor { get; set; } = Color.Parse("#1BD76A");
    [Reactive] [JsonProperty] public Color BackGroundColor { get; set; } = Color.Parse("#00B7FF52");
    [Reactive] [JsonProperty] public bool UseFilePicker { get; set; } = true;
    [Reactive] [JsonProperty] public bool EnableBottomContainer { get; set; } = true;
    [Reactive] [JsonProperty] public bool AutoCheckUpdate { get; set; } = true;
    [Reactive] [JsonProperty] public bool EnableSpeedUpGithubApi { get; set; } = true;
    [Reactive] [JsonProperty] public string PoemApiToken { get; set; }
    [Reactive] [JsonProperty] public string GithubSpeedUpApiUrl { get; set; } = "https://ghproxy.net/%url%";
    [Reactive] [JsonProperty] public string BackGroundImgData { get; set; }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }
}