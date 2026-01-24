using System;
using System.IO;

namespace Mirel.Const;

public static class ConfigPath
{
    private static readonly string _sessionTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    public static string UserDataRootPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "yiiko.Mirel");
    public static string TempFolderPath => Path.Combine(UserDataRootPath, "Mirel.Temp");

    public static string SettingDataPath => Path.Combine(UserDataRootPath, "Mirel.Setting.yiiko");
    public static string AppPathDataPath => Path.Combine(UserDataRootPath, "Mirel.AppPath.yiiko");
    public static string PluginFolderPath => Path.Combine(UserDataRootPath, "Mirel.Plugins");
    public static string PluginUnzipFolderPath => Path.Combine(PluginFolderPath, "Mirel.UnzipPlugins");
    public static string PluginTempFolderPath => Path.Combine(PluginUnzipFolderPath, _sessionTimestamp);
    public static string PluginDebugFolderPath => Path.Combine(PluginFolderPath, "Mirel.DebugPlugins");
}