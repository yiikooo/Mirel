using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Mirel.Const;

public class Data : ReactiveObject
{
    private static Data? _instance;

    public static Data Instance
    {
        get { return _instance ??= new Data(); }
    }

    public static RunnerType RunnerType { get; set; } = RunnerType.Unknown;
    public static DesktopType DesktopType { get; set; } = DesktopType.Unknown;
    public static SettingEntry SettingEntry { get; set; }
    public static UiProperty UiProperty { get; set; } = UiProperty.Instance;
    public static string TranslateToken { get; set; }
    [Reactive] public string Version { get; set; }
}