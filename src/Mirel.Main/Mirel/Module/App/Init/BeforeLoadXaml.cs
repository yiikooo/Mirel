using Mirel.Const;
using Mirel.Module.App.Init.Config;
using Mirel.Module.Event;
using Mirel.Module.IO.Local;
using Update = Mirel.Module.App.Init.Config.Update;

namespace Mirel.Module.App.Init;

public abstract class BeforeLoadXaml
{
    public static void Main()
    {
        Sundry.DetectPlatform();
        Create.Main();
        Setter.TryClearFolder(ConfigPath.PluginUnzipFolderPath);
        InitializationEvents.RaiseBeforeReadSettings();
        Reader.Main();
        Update.Main();
        InitializationEvents.RaiseBeforeUiLoaded();
    }
}