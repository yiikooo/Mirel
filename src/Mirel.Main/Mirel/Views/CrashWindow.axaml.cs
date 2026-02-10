using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Platform;
using HotAvalonia;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.App;
using Mirel.Module.App.Services;
using Mirel.Module.Ui;
using Mirel.Module.Ui.Helper;
using Ursa.Controls;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace Mirel.Views;

public partial class CrashWindow : UrsaWindow, IMirelWindow, INotifyPropertyChanged
{
    private string _exception;

    public CrashWindow(string exception)
    {
        InitializeComponent();
        DataContext = this;
        Info = exception;
        Init();
    }

    public CrashWindow(Exception e)
    {
        InitializeComponent();
        DataContext = this;
        Info = e.ToString();
        Init();
    }

    public CrashWindow()
    {
        InitializeComponent();
        Init();
    }

    public string Info
    {
        get => _exception;
        set => SetField(ref _exception, value);
    }

    public WindowNotificationManager Notification { get; set; }
    public MirelWindowToastManager Toast { get; set; }
    public Control RootElement { get; set; }
    public UrsaWindow Window { get; set; }

    public sealed override void Show()
    {
        base.Show();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

#if DEBUG
    [AvaloniaHotReload]
#endif
    public void Init()
    {
        Setter.UpdateWindowStyle(this);
        Copy.Click += async (_, _) =>
        {
            var clipboard = GetTopLevel(this)?.Clipboard;
            await clipboard.SetTextAsync(_exception);
        };
        Continue.Click += (_, _) => { Close(); };
        Restart.Click += (_, _) => { AppMethod.RestartApp(); };
        Exit.Click += (_, _) => { AppMethod.TryExitApp(); };
        Topmost = true;
        Loaded += (_, _) => { Setter.UpdateWindowStyle(this); };
        MoreInfo.Click += async (_, _) =>
        {
            var q =
                string.Join("", _exception
                        .Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
                        .Take(2))
                    .Replace(
                        "System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.",
                        "");
            var launcher = GetTopLevel(this)?.Launcher;
            await launcher.LaunchUriAsync(new Uri($"https://www.bing.com/search?q={q}"));
        };
        Notification = new WindowNotificationManager(GetTopLevel(this));
        Toast = new MirelWindowToastManager(GetTopLevel(this));
        Notification.Position = NotificationPosition.BottomRight;
        RootElement = Root;
        Window = this;
        Show();
        Activate();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        BindingKeys.Main(this);

        if (Data.DesktopType == DesktopType.Linux ||
            Data.DesktopType == DesktopType.FreeBSD ||
            (Data.DesktopType == DesktopType.Windows &&
             Environment.OSVersion.Version.Major < 10))
        {
            IsManagedResizerVisible = true;
            SystemDecorations = SystemDecorations.None;
            Root.CornerRadius = new CornerRadius(0);
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
        else if (Data.DesktopType == DesktopType.MacOs)
        {
            SystemDecorations = SystemDecorations.Full;
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
            ExtendClientAreaToDecorationsHint = true;
            TitleRoot.Margin = new Thickness(65, 0, 0, 0);
            TitleBar.IsCloseBtnShow = false;
            TitleBar.IsMinBtnShow = false;
            TitleBar.IsMaxBtnShow = false;
        }
        else
        {
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }

        if (Data.DesktopType == DesktopType.MacOs)
            PropertyChanged += (_, _) =>
            {
                var platform = TryGetPlatformHandle();
                if (platform is null) return;
                var nsWindow = platform.Handle;
                if (nsWindow == IntPtr.Zero) return;
                try
                {
                    MacOsWindowHandler.RefreshTitleBarButtonPosition(nsWindow);
                    MacOsWindowHandler.HideZoomButton(nsWindow);

                    ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            };
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}