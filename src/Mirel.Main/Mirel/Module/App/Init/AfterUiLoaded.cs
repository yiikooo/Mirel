using System.Diagnostics;
using System.IO;
using Mirel.Const;
using Mirel.Module.App.Services;
using Mirel.Module.Event;
using Mirel.Module.Ui;
using Mirel.Views.Main.Components;

namespace Mirel.Module.App.Init;

public abstract class AfterUiLoaded
{
    public static void Main()
    {
        PageNav.Initialize();
        File.WriteAllText(ConfigPath.AppPathDataPath,
            Process.GetCurrentProcess().MainModule.FileName);
        BindingAppEvents.Main();
        BindingKeys.Main(Mirel.App.UiRoot!);
        Setter.SetAccentColor(Data.SettingEntry.ThemeColor);
        Setter.ToggleTheme(Data.SettingEntry.Theme);
        LoopGC.BeginLoop();
        AppExit.Main();
        InitializationEvents.RaiseAfterUiLoaded();
    }
}