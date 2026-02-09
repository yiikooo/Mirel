using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Events;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class TabsPage : PageModelBase, IMirelPage
{
    public TabsPage()
    {
        InitializeComponent();
        RootElement = Root;
        ListBox.SelectionChanged += (_, _) =>
        {
            if (ListBox.SelectedItem is not TabSEntry tab) return;
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime?.Windows == null) return;
            var windows = lifetime.Windows
                .OfType<IMirelTabWindow>()
                .ToList();
            foreach (var window in windows.Where(window => window.Tabs.Contains(tab.Entry)))
            {
                window.SelectTab(tab.Entry);
                window.Activate();
                break;
            }
        };
        AppEvents.TabSelectionChanged += e => { ListBox.SelectedItem = e; };
        Loaded += (_, _) =>
        {
            if (UiProperty.ActiveWindow is not IMirelTabWindow tb) return;
            ListBox.SelectedItem = new TabSEntry
            {
                Entry = tb.SelectedTab,
                Window = tb
            };
        };
    }

    public Control RootElement { get; init; }

    private void TabContextMenu_Opening(object? sender, EventArgs e)
    {
        if (sender is not MenuFlyout menuFlyout) return;
        if (menuFlyout.Target is not DockPanel dockPanel) return;
        if (dockPanel.DataContext is not TabSEntry tabSEntry) return;

        var tabEntry = tabSEntry.Entry;
        var sourceWindow = tabSEntry.Window;

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
                    if (sourceWindow is TabWindow tabWindow)
                    {
                        tabWindow.RemoveTab(tabEntry);
                    }
                    else
                    {
                        sourceWindow.Tabs.Remove(tabEntry);
                    }

                    tabEntry.DisposeContent();
                    tabEntry.Removing();
                }
            });
            closeTabMenuItem.IsEnabled = tabEntry.CanClose;
        }

        // Setup Open in New Window command
        if (openInNewWindowMenuItem != null)
        {
            openInNewWindowMenuItem.Command = new RelayCommand(() =>
            {
                var newWindow = new TabWindow();
                if (sourceWindow is TabWindow tabWindow)
                {
                    tabWindow.RemoveTab(tabEntry);
                }
                else
                {
                    sourceWindow.Tabs.Remove(tabEntry);
                }

                tabEntry.RefreshContent();
                newWindow.CreateTab(tabEntry);
                newWindow.Show();
                newWindow.Activate();
            });
        }

        // Setup Move to Window submenu
        if (moveToWindowMenuItem != null)
        {
            moveToWindowMenuItem.Items.Clear();

            var windows = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.Windows.OfType<TabWindow>().ToArray() ?? Array.Empty<TabWindow>();

            var otherWindows = windows.Where(w => w != sourceWindow).ToArray();

            if (otherWindows.Length == 0)
            {
                var noWindowItem = new MenuItem
                {
                    Header = "没有其他窗口",
                    IsEnabled = false
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
                        Command = new RelayCommand(() =>
                        {
                            if (sourceWindow is TabWindow sourceTabWindow)
                            {
                                sourceTabWindow.RemoveTab(tabEntry);
                            }
                            else
                            {
                                sourceWindow.Tabs.Remove(tabEntry);
                            }

                            tabEntry.RefreshContent();
                            window.CreateTab(tabEntry);
                            window.Activate();
                        })
                    };
                    moveToWindowMenuItem.Items.Add(menuItem);
                }
            }
        }
    }
}