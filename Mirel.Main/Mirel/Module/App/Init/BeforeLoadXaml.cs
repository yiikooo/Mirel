using Mirel.Const;
using Mirel.Module.App.Init.Config;
using Mirel.Module.App.Services;
using Mirel.Module.Events;
using Update = Mirel.Module.App.Init.Config.Update;

namespace Mirel.Module.App.Init;

public abstract class BeforeLoadXaml
{
    public static void Main()
    {
        Sundry.DetectPlatform();
        Create.Main();
        IO.Local.Setter.TryClearFolder(ConfigPath.PluginUnzipFolderPath);
        InitEvents.OnBeforeReadSettings();
        Reader.Main();
        Update.Main();
        InitEvents.OnBeforeUiLoaded();
    }
}