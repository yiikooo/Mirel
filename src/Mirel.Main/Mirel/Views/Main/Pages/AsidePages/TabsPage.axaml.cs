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
using Mirel.Module.Ui.Helper;
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

        TabContextMenuBuilder.BuildContextMenu(menuFlyout, tabEntry, sourceWindow);
    }
}