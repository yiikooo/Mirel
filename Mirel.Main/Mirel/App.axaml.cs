using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.Core;
using Mirel.Module;
using Mirel.Module.App.Init;
using Mirel.ViewModels;
using Mirel.Views.Main;

namespace Mirel;

public partial class App : Application
{
  public delegate void UiLoadedEventHandler(MainWindow ui);

    private bool _fl = true;

    public static MainWindow? UiRoot => (Current!.ApplicationLifetime
        as IClassicDesktopStyleApplicationLifetime).MainWindow as MainWindow;

    public static TopLevel TopLevel => TopLevel.GetTopLevel(UiRoot);
    public static event UiLoadedEventHandler UiLoaded;

    public override void Initialize()
    {
        FAUISettings.SetAnimationsEnabledAtAppLevel(false);
        Logger.Info("开始初始化");
        BeforeLoadXaml.Main();
        AvaloniaXamlLoader.Load(this);
        Logger.Info("完成初始化");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Logger.Info("初始化Framework");
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
#if DEBUG
            Logger.Debug("挂载 Devtools");
            this.AttachDevTools();
#endif
            DisableAvaloniaDataAnnotationValidation();

#if RELEASE
            Logger.Info("注册全局异常处理");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UIThread.UnhandledException += UIThread_UnhandledException;
#else
            Dispatcher.UIThread.UnhandledException += (_, e) =>
            {
                Logger.Fatal($"UI线程异常: {e.Exception}");
                throw e.Exception;
            };
#endif
            var win = new MainWindow();
            desktop.MainWindow = win;
            win.Loaded += (_, _) =>
            {
                if (!_fl) return;
                Logger.Info("UI加载完成");
                AfterUiLoaded.Main();
                UiLoaded?.Invoke(win);
                _fl = false;
            };
            Logger.Info("UI配置完成");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void UIThread_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Logger.Fatal($"UI线程异常: {e.Exception}");
        try
        {
            // var win = new CrashWindow(e.Exception.ToString());
            // win.Show();
        }
        catch (Exception ex)
        {
            Logger.Fatal($"显示崩溃窗口失败: {ex}");
        }
        finally
        {
            e.Handled = true;
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Fatal($"AppDomain 异常: {e}");
        // try
        // {
        //     var win = new CrashWindow(e.ToString() ?? "Unhandled Exception");
        //     win.Show();
        // }
        // catch (Exception ex)
        // {
        //     Logger.Fatal($"{MainLang.ShowCrashWindowFailTip}: {ex}");
        // }
        //TODO 显示崩溃窗口
    }


    private void DisableAvaloniaDataAnnotationValidation()
    {
        Logger.Debug("DisableAvaloniaDataAnnotation");
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}