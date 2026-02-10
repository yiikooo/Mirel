using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HotAvalonia;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module;
using Mirel.Module.Event;
using Mirel.Module.Service;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Mirel.Views.Main.Pages;
using Ursa.Controls;
using MoreButtonMenu = Mirel.Views.Main.Components.MoreButtonMenu;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace Mirel.Views.Main;

public partial class MainWindow : UrsaWindow, IMirelTabWindow
{
    private bool _isShiftKeyDown;
    private DateTime _lastShiftPressTime;
    private DateTime _shiftKeyDownTime;

    public MainWindow()
    {
#if DEBUG
        InitializeComponent(attachDevTools: false);
#else
        InitializeComponent();
#endif
        Notification = new WindowNotificationManager(GetTopLevel(this));
        Toast = new MirelWindowToastManager(GetTopLevel(this));
        Notification.Position = NotificationPosition.BottomRight;
        RootElement = Root;
        Toast.MaxItems = 4;
        DataContext = ViewModel;
        Window = this;
        TabDragDropService.RegisterWindow(this);
#if RELEASE
        BindEvents();
        InitTitleBar();
#endif
    }

    public MainViewModel ViewModel { get; } = new();
    public ObservableCollection<TabEntry> Tabs => ViewModel.Tabs;
    public TabEntry SelectedTab => ViewModel.SelectedTab;
    public string WindowId { get; init; } = "主窗口";

    public void SelectTab(TabEntry tab)
    {
        if (tab == null) return;
        if (!Tabs.Contains(tab)) return;
        ViewModel.SelectedTab = tab;
    }

    public WindowNotificationManager Notification { get; set; }
    public MirelWindowToastManager Toast { get; set; }
    public Control RootElement { get; set; }
    public UrsaWindow Window { get; set; }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

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
            Separator.IsVisible = false;
        }
        else
        {
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
    }

#if DEBUG
    [AvaloniaHotReload]
#endif
    private void InitTitleBar()
    {
        var c = new MoreButtonMenu();
        var menu = (MenuFlyout)c.MainControl!.Flyout;
        TitleRoot.ContextFlyout = menu;
        TitleRoot.DataContext = new MoreButtonMenuCommands();
        NewTabButton.Click += (_, _) => { CreateTab(new TabEntry(new NewTabPage())); };
        NavRoot.Margin = new Thickness(Data.DesktopType == DesktopType.MacOs ? 100 : 40, 0,
            TitleBarContainer.Bounds.Width + 40 + (Data.DesktopType == DesktopType.MacOs ? 20 : 85), 0);
        TitleRoot.PointerPressed += (_, e) => { TitleRoot.ContextFlyout.ShowAt(TitleRoot); };
        TitleBarContainer.SizeChanged += (_, _) =>
        {
            NavRoot.Margin = new Thickness(Data.DesktopType == DesktopType.MacOs ? 100 : 40, 0,
                TitleBarContainer.Bounds.Width + 40 + (Data.DesktopType == DesktopType.MacOs ? 20 : 85), 0);
        };
    }

#if DEBUG
    [AvaloniaHotReload]
#endif
    private void BindEvents()
    {
        ViewModel.Tabs.CollectionChanged += (_, _) => { TabService.UpdateTabs(); };
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.SelectedTab))
                AppEvents.OnTabSelectionChanged(new TabSEntry
                {
                    Entry = SelectedTab,
                    Window = this
                });
        };
        Activated += (_, _) =>
        {
            AppEvents.OnTabSelectionChanged(new TabSEntry
            {
                Entry = SelectedTab,
                Window = this
            });
        };
        Closing += OnMainWindowClosing;
        if (Data.DesktopType == DesktopType.MacOs)
            PropertyChanged += (_, e) =>
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
                    Logger.Error(exception);
                }
            };

        ASideButton.Click += (_, _) => { Data.SettingEntry.EnableAside = !Data.SettingEntry.EnableAside; };
        NavScrollViewer.ScrollChanged += (_, _) => { ViewModel.IsTabMaskVisible = NavScrollViewer.Offset.X > 0; };
        Loaded += (_, _) =>
        {
            RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
            RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
            RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        };
        KeyDown += (_, e) =>
        {
            // Logger.Info("KeyDown: " + e.Key);
            if (e.Key is not (Key.LeftShift or Key.RightShift)) return;

            // Record when shift key is pressed down
            if (!_isShiftKeyDown)
            {
                _shiftKeyDownTime = DateTime.Now;
                _isShiftKeyDown = true;
            }
        };

        KeyUp += (_, e) =>
        {
            if (e.Key is not (Key.LeftShift or Key.RightShift)) return;
            if (!_isShiftKeyDown) return;

            _isShiftKeyDown = false;
            var keyHoldDuration = (DateTime.Now - _shiftKeyDownTime).TotalMilliseconds;

            // Only consider it a valid tap if the key was held for less than 200ms (quick tap)
            if (keyHoldDuration < 200)
            {
                var timeSinceLastTap = (DateTime.Now - _lastShiftPressTime).TotalMilliseconds;

                // Check if this is a double tap within 300ms
                if (timeSinceLastTap < 300)
                {
                    var options = new DialogOptions
                    {
                        ShowInTaskBar = false,
                        IsCloseButtonVisible = true,
                        StartupLocation = WindowStartupLocation.Manual,
                        CanDragMove = true,
                        StyleClass = "aggregate-search"
                    };
                    // Dialog.ShowCustom<AggregateSearchDialog, AggregateSearchDialog>(new AggregateSearchDialog(),
                    //     this.GetVisualRoot() as Window, options: options); //TODO
                }

                _lastShiftPressTime = DateTime.Now;
            }
        };
        AddHandler(DragDrop.DropEvent, DropHandler);
    }

    private void DropHandler(object? sender, DragEventArgs e)
    {
        DataDragDropService.HandleData(this, e);
    }

    private void TabItem_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed) return;
        var c = (TabEntry)((Border)sender).Tag;
        c.Close();
    }

    private void NavScrollViewer_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            scrollViewer.Offset = new Vector(
                scrollViewer.Offset.X + e.Delta.Y * -20, // 调整乘数以控制滚动速度
                scrollViewer.Offset.Y
            );
            e.Handled = true;
        }
    }

    public void CreateTab(TabEntry tab)
    {
        ViewModel.CreateTab(tab);
    }

    public void TogglePage(string tag, IMirelTabPage page)
    {
        var existingTab = Tabs.FirstOrDefault(x => x.Tag == tag);

        if (existingTab == null || tag == null)
        {
            var newTab = new TabEntry(page)
            {
                Tag = tag
            };
            Tabs.Add(newTab);
            ViewModel.SelectedTab = newTab;
        }
        else
        {
            if (SelectedTab == existingTab)
            {
                existingTab.Content.InAnimator.Animate();
                return;
            }

            ViewModel.SelectedTab = existingTab;
        }
    }

    private static async void OnMainWindowClosing(object? sender, WindowClosingEventArgs _)
    {
        try
        {
            if (!await AppEvents.OnAppExiting()) return;
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            ExceptionService.HandleException(e);
        }
    }

    [GeneratedRegex(@"https?://[^\s:]+")]
    private static partial Regex MyRegex();

    private void TabContextMenu_Opening(object? sender, EventArgs e)
    {
        if (sender is not MenuFlyout menuFlyout) return;
        if (menuFlyout.Target is not Border border) return;
        if (border.Tag is not TabEntry tabEntry) return;

        TabContextMenuBuilder.BuildContextMenu(menuFlyout, tabEntry, this);
    }
}