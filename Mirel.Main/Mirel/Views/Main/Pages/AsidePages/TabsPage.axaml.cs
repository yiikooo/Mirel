using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Events;
using Mirel.Module.Service;
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

            // Exclude current source window
            var otherWindows = allWindows.Where(w => w != sourceWindow).ToList();

            if (otherWindows.Count == 0)
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
                        Icon = new PathIcon
                        {
                            Data = Geometry.Parse(
                                "F1 M512,512z M0,0z M502.6,278.6C515.1,266.1,515.1,245.8,502.6,233.3L342.6,73.3C330.1,60.8 309.8,60.8 297.3,73.3 284.8,85.8 284.8,106.1 297.3,118.6L402.7,224 32,224C14.3,224 0,238.3 0,256 0,273.7 14.3,288 32,288L402.7,288 297.3,393.4C284.8,405.9 284.8,426.2 297.3,438.7 309.8,451.2 330.1,451.2 342.6,438.7L502.6,278.7z"),
                            Width = 17
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