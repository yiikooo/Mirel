using Mirel.Const;
using Mirel.Module.IO.Local;

namespace Mirel.Module.App.Init.Config;

public class Update
{
    public static void Main()
    {
        Setter.ClearFolder(ConfigPath.TempFolderPath);
        AppMethod.SaveSetting();
    }
}