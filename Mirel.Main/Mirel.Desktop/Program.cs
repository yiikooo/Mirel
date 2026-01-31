using System;
using Avalonia;
using Mirel.Module;

namespace Mirel.Desktop;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // 初始化日志系统
        Logger.Initialize();
        Logger.Info("应用程序启动");
        
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new MacOSPlatformOptions
            {
                DisableAvaloniaAppDelegate = true,
                DisableDefaultApplicationMenuItems = true
            })
            .LogToTrace();
}