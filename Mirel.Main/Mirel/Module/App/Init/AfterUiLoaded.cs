using System.Diagnostics;
using System.IO;
using Avalonia;
using Mirel.Classes.Enums;
using Mirel.Const;
using Mirel.Module.App.Services;
using Mirel.Module.Events;
using Mirel.Module.Ui;

namespace Mirel.Module.App.Init;

public abstract class AfterUiLoaded
{
    public static void Main()
    {
        File.WriteAllText(ConfigPath.AppPathDataPath,
            Process.GetCurrentProcess().MainModule.FileName);
        BindingAppEvents.Main();
        BindingKeys.Main(Mirel.App.UiRoot!);
        Setter.SetAccentColor(Data.SettingEntry.ThemeColor);
        Application.Current.Resources["BackGroundOpacity"] = Data.SettingEntry.BackGround == Setting.BackGround.Default ? 1.0 : 0.5;
        Setter.ToggleTheme(Data.SettingEntry.Theme);
        LoopGC.BeginLoop();
        // if (Data.SettingEntry.AutoCheckUpdate && Data.Instance.Version != "vDebug")
        //     _ = MirelPage.ShowUpdateDialogIfNeed(); //TODO: update
        PageNav.Main();
        AppExit.Main();
        InitEvents.OnAfterUiLoaded();
    }
}