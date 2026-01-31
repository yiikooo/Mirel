using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Events;
using Mirel.Module.Service;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Mirel.Views.Main.Pages;
using Ursa.Controls;
using TitleBar = Mirel.Controls.TitleBar;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace Mirel.Views.Main;

public partial class MainWindow : UrsaWindow, IMirelWindow
{
    public TitleBar WindowTitleBar => this.TitleBar;

    public MainWindow()
    {
#if DEBUG
        InitializeComponent(attachDevTools: false);
#else
        InitializeComponent();
#endif
        Notification = new WindowNotificationManager(GetTopLevel(this));
        Toast = new WindowToastManager(GetTopLevel(this));
        Notification.Position = NotificationPosition.BottomRight;
        RootElement = Root;
        Toast.MaxItems = 2;
        DataContext = ViewModel;
        Window = this;
        TabDragDropService.RegisterWindow(this);
        BindEvents();
        InitTitleBar();
    }

    public MainViewModel ViewModel { get; set; } = new();
    public ObservableCollection<TabEntry> Tabs => ViewModel.Tabs;
    private DateTime _lastShiftPressTime;
    private DateTime _shiftKeyDownTime;
    private bool _isShiftKeyDown;
    public TabEntry? SelectedTab => ViewModel.SelectedTab;

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
        }
        else
        {
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
    }

    private void InitTitleBar()
    {
        var c = new MoreButtonMenu();
        var menu = (MenuFlyout)c.MainControl!.Flyout;
        TitleRoot.ContextFlyout = menu;
        TitleRoot.DataContext = new MoreButtonMenuCommands();
        NewTabButton.Click += (_, _) => { CreateTab(new TabEntry(new NewTabPage())); };
    }

    private void OpenSettingPage(int pageIndex = -1)
    {
        var existingTab = Tabs.FirstOrDefault(x => x.Tag == "setting");

        if (existingTab == null)
        {
            var settingTabPage = new SettingTabPage();


            var newTab = new TabEntry(settingTabPage)
            {
                Tag = "setting"
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

    private void BindEvents()
    {
        Closing += OnMainWindowClosing;
        if (Data.DesktopType == DesktopType.MacOs)
        {
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
                    Console.WriteLine(exception);
                }
            };
        }

        NavScrollViewer.ScrollChanged += (_, _) => { ViewModel.IsTabMaskVisible = NavScrollViewer.Offset.X > 0; };
        Loaded += (_, _) =>
        {
            RenderOptions.SetTextRenderingMode(this, TextRenderingMode.SubpixelAntialias); // 字体渲染模式
            RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.MediumQuality); // 图片渲染模式
            RenderOptions.SetEdgeMode(this, EdgeMode.Antialias); // 形状渲染模式
        };
        TitleBar.SizeChanged += (_, _) =>
        {
            NavRoot.Margin = new Thickness((Data.DesktopType == DesktopType.MacOs ? 125 : 80), 0,
                70 + (Data.DesktopType == DesktopType.MacOs ? 20 : 85), 0);
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
                    var options = new DialogOptions()
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

    private async void DropHandler(object? sender, DragEventArgs e)
    {
        if (e is null) return;
        AppEvents.OnAppDragDrop(sender, e);
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles();
            if (files == null) return;
            foreach (var file in files)
            {
                //TODO
            }
        }
        else if (e.Data.Contains(DataFormats.Text))
        {
            var text = e.Data.GetText();
        }
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
                scrollViewer.Offset.X + e.Delta.Y * 20, // 调整乘数以控制滚动速度
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

    private async void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        // If this is the main window closing and there are other TabWindows open,
        // we should handle the scenario appropriately
        if (!await AppEvents.OnAppExiting()) return;
        TabDragDropService.UnregisterWindow(this);
        Environment.Exit(0);
    }

    public WindowNotificationManager Notification { get; set; }
    public WindowToastManager Toast { get; set; }
    public Control RootElement { get; set; }
    public UrsaWindow Window { get; set; }

    [GeneratedRegex(@"https?://[^\s:]+")]
    private static partial Regex MyRegex();
}