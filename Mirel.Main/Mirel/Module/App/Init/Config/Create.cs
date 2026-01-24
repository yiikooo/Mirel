using System.IO;
using Mirel.Classes.Entries;
using Mirel.Const;
using Mirel.Module.IO.Local;

namespace Mirel.Module.App.Init.Config;

public abstract class Create
{
    public static void Main()
    {
        Folder();
        Data();
    }

    public static void Data()
    {
        if (!File.Exists(ConfigPath.SettingDataPath))
            File.WriteAllText(ConfigPath.SettingDataPath, new SettingEntry().AsJson());
    }

    public static void Folder()
    {
        Setter.TryCreateFolder(ConfigPath.UserDataRootPath);
        Setter.TryCreateFolder(ConfigPath.PluginFolderPath);
        Setter.TryCreateFolder(ConfigPath.TempFolderPath);
        Setter.TryCreateFolder(ConfigPath.PluginUnzipFolderPath);
        Setter.TryCreateFolder(ConfigPath.PluginTempFolderPath);
        Setter.TryCreateFolder(ConfigPath.PluginDebugFolderPath);
    }
}