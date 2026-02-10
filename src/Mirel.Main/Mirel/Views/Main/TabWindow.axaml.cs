using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module;
using Mirel.Module.App.Services;
using Mirel.Module.Events;
using Mirel.Module.Service;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Mirel.Views.Main.Pages;
using Ursa.Controls;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace Mirel.Views.Main;

public partial class TabWindow : UrsaWindow, IMirelTabWindow
{
    private bool _isShiftKeyDown;
    private DateTime _lastShiftPressTime;
    private DateTime _shiftKeyDownTime;

    public TabWindow()
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
        Window = this;
        Toast.MaxItems = 4;
        DialogHost.HostId = $"DialogHost_{DateTime.Now}";
        DataContext = ViewModel;
        NewTabButton.DataContext = ViewModel;
        BindEvents();
        TabDragDropService.RegisterWindow(this);
        Code.Text = $"{WindowId}";
    }

    public string HostId => DialogHost.HostId;

    public TabWindowViewModel ViewModel { get; set; } = new();
    public ObservableCollection<TabEntry> Tabs => ViewModel.Tabs;
    public TabEntry SelectedTab => ViewModel.SelectedTab;
    public string WindowId { get; init; } = "#" + Guid.NewGuid().ToString()[..6];

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
            NavRoot.Margin = new Thickness(60, 0, 130, 0);
        }
        else
        {
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaToDecorationsHint = true;
        }
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

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Transfer remaining tabs back to main window if any exist
        if (ViewModel.HasTabs && App.UiRoot != null)
        {
            var tabsToTransfer = Tabs.ToList();

            // Use dispatcher to ensure proper UI thread handling
            Dispatcher.UIThread.Post(async () =>
            {
                foreach (var tab in tabsToTransfer)
                {
                    ViewModel.RemoveTab(tab);

                    // Small delay and refresh to avoid layout conflicts
                    await Task.Delay(25);
                    tab.RefreshContent();

                    App.UiRoot.ViewModel.CreateTab(tab);
                }
            });
        }

        TabDragDropService.UnregisterWindow(this);
    }

    private void BindEvents()
    {
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
        ViewModel.Tabs.CollectionChanged += (_, _) =>
        {
            if (ViewModel.Tabs.Count == 0)
            {
                Close();
                return;
            }

            TabService.UpdateTabs();
        };
        NewTabButton.Click += NewTabButton_Click;
        Closing += OnClosing;
        ViewModel.TabsEmptied += OnTabsEmptied;
        NavScrollViewer.ScrollChanged += (_, _) => { ViewModel.IsTabMaskVisible = NavScrollViewer.Offset.X > 0; };
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

        KeyDown += (_, e) =>
        {
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
        c.CloseInWindow(this);
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

    public void AddTab(TabEntry tab)
    {
        ViewModel.AddTab(tab);
    }

    public void RemoveTab(TabEntry tab)
    {
        ViewModel.RemoveTab(tab);
    }

    private void OnTabsEmptied()
    {
        // Close the window when no tabs remain
        Close();
    }

    private void NewTabButton_Click(object? sender, RoutedEventArgs e)
    {
        CreateTab(new TabEntry(new NewTabPage()));
    }

    private void TabContextMenu_Opening(object? sender, EventArgs e)
    {
        if (sender is not MenuFlyout menuFlyout) return;
        if (menuFlyout.Target is not Border border) return;
        if (border.Tag is not TabEntry tabEntry) return;

        // 使用新的菜单构建器
        TabContextMenuBuilder.BuildContextMenu(menuFlyout, tabEntry, this);
    }
}