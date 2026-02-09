using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Views.Main;

namespace Mirel.Module.Ui.Helper;

public static class SingletonTabManager
{
    public static void CreateOrActivateSingletonTab(IMirelTabPage page)
    {
        if (page is not IMirelSingletonTabPage) return;

        var pageType = page.GetType();
        var existingTab = FindExistingTab(pageType);

        if (existingTab != null)
        {
            MoveTabToMainWindow(existingTab);
        }
        else
        {
            Mirel.App.UiRoot.CreateTab(new TabEntry(page));
        }
    }

    private static TabEntry? FindExistingTab(Type pageType)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime?.Windows == null) return null;

        if (lifetime.MainWindow is MainWindow mainWindow)
        {
            var tab = mainWindow.Tabs.FirstOrDefault(t => t.Content?.GetType() == pageType);
            if (tab != null) return tab;
        }

        var tabWindows = lifetime.Windows.OfType<TabWindow>();
        return tabWindows.Select(window => window.Tabs.FirstOrDefault(t => t.Content?.GetType() == pageType))
            .OfType<TabEntry>().FirstOrDefault();
    }

    private static void MoveTabToMainWindow(TabEntry tab)
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime?.Windows == null) return;

        var currentWindow = lifetime.Windows.OfType<IMirelTabWindow>()
            .FirstOrDefault(w => w.Tabs.Contains(tab));

        if (currentWindow == null) return;

        if (currentWindow is MainWindow)
        {
            Mirel.App.UiRoot.SelectTab(tab);
            Mirel.App.UiRoot.Activate();
        }
        else if (currentWindow is TabWindow tabWindow)
        {
            tabWindow.ViewModel.RemoveTab(tab);
            tab.RefreshContent();
            Mirel.App.UiRoot.ViewModel.CreateTab(tab);
            Mirel.App.UiRoot.SelectTab(tab);
            Mirel.App.UiRoot.Activate();
        }
    }
}