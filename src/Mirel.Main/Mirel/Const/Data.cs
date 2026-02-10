using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Module.Service;
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

    public static DesktopType DesktopType { get; set; } = DesktopType.Unknown;
    public static SettingEntry SettingEntry { get; set; }
    public static UiProperty UiProperty { get; } = UiProperty.Instance;
    public static TabService TabService { get; } = TabService.Instance;
    [Reactive] public string Version { get; set; }
}