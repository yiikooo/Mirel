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

        // Find menu items
        MenuItem? closeTabMenuItem = null;
        MenuItem? openInNewWindowMenuItem = null;
        MenuItem? moveToWindowMenuItem = null;

        foreach (var item in menuFlyout.Items)
        {
            if (item is MenuItem menuItem)
            {
                if (menuItem.Name == "CloseTabMenuItem")
                    closeTabMenuItem = menuItem;
                else if (menuItem.Name == "OpenInNewWindowMenuItem")
                    openInNewWindowMenuItem = menuItem;
                else if (menuItem.Name == "MoveToWindowMenuItem")
                    moveToWindowMenuItem = menuItem;
            }
        }

        // Setup Close Tab command
        if (closeTabMenuItem != null)
        {
            closeTabMenuItem.Command = new RelayCommand(() =>
            {
                if (tabEntry.CanClose)
                {
                    RemoveTab(tabEntry);
                    tabEntry.DisposeContent();
                    tabEntry.Removing();
                }
            });
            closeTabMenuItem.IsEnabled = tabEntry.CanClose;
        }

        // Setup Open in New Window command - use the drag service method
        if (openInNewWindowMenuItem != null)
        {
            openInNewWindowMenuItem.Command = new RelayCommand(() =>
            {
                // Use the MoveTabToNewWindow extension method from TabDragDropService
                tabEntry.MoveTabToNewWindow();
            });
        }

        // Setup Move to Window submenu
        if (moveToWindowMenuItem != null)
        {
            moveToWindowMenuItem.Items.Clear();

            // Get all tab windows (both MainWindow and TabWindow)
            var allWindows = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.Windows.OfType<IMirelTabWindow>().ToList() ?? new List<IMirelTabWindow>();

            // Exclude current window
            var otherWindows = allWindows.Where(w => w != this).ToList();

            if (otherWindows.Count == 0)
            {
                var noWindowItem = new MenuItem
                {
                    Header = "没有其他窗口",
                    IsEnabled = false, Icon = new PathIcon
                    {
                        Data = Geometry.Parse(
                            "F1 M512,512z M0,0z M502.6,278.6C515.1,266.1,515.1,245.8,502.6,233.3L342.6,73.3C330.1,60.8 309.8,60.8 297.3,73.3 284.8,85.8 284.8,106.1 297.3,118.6L402.7,224 32,224C14.3,224 0,238.3 0,256 0,273.7 14.3,288 32,288L402.7,288 297.3,393.4C284.8,405.9 284.8,426.2 297.3,438.7 309.8,451.2 330.1,451.2 342.6,438.7L502.6,278.7z"),
                        Width = 14, Margin = new Thickness(8, -1, 0, 0)
                    },
                };
                moveToWindowMenuItem.Items.Add(noWindowItem);
            }
            else
            {
                foreach (var window in otherWindows)
                {
                    var menuItem = new MenuItem
                    {
                        Header = window.WindowId,
                        Icon = new PathIcon
                        {
                            Data = Geometry.Parse(
                                "F1 M512,512z M0,0z M502.6,278.6C515.1,266.1,515.1,245.8,502.6,233.3L342.6,73.3C330.1,60.8 309.8,60.8 297.3,73.3 284.8,85.8 284.8,106.1 297.3,118.6L402.7,224 32,224C14.3,224 0,238.3 0,256 0,273.7 14.3,288 32,288L402.7,288 297.3,393.4C284.8,405.9 284.8,426.2 297.3,438.7 309.8,451.2 330.1,451.2 342.6,438.7L502.6,278.7z"),
                            Width = 14, Margin = new Thickness(8, -1, 0, 0)
                        },
                        Command = new RelayCommand(() =>
                        {
                            // Use the MoveTabToWindow extension method from TabDragDropService
                            tabEntry.MoveTabToWindow(window as Window);
                        })
                    };
                    moveToWindowMenuItem.Items.Add(menuItem);
                }
            }
        }
    }
}