using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Views.Main;

namespace Mirel.ViewModels;

public class TabContextMenuCommands : ViewModelBase
{
    private IMirelTabWindow _sourceWindow;
    private TabEntry _targetTab;

    public TabEntry TargetTab
    {
        get => _targetTab;
        set => SetField(ref _targetTab, value);
    }

    public IMirelTabWindow SourceWindow
    {
        get => _sourceWindow;
        set => SetField(ref _sourceWindow, value);
    }

    public void CloseTab()
    {
        if (TargetTab == null || !TargetTab.CanClose) return;

        if (SourceWindow is TabWindow tabWindow)
        {
            tabWindow.RemoveTab(TargetTab);
            TargetTab.DisposeContent();
            TargetTab.Removing();
        }
    }

    public void OpenInNewWindow()
    {
        if (TargetTab == null) return;

        // Create new TabWindow
        var newWindow = new TabWindow();

        // Remove from current window
        if (SourceWindow is TabWindow tabWindow)
        {
            tabWindow.RemoveTab(TargetTab);
        }
        else if (App.UiRoot != null)
        {
            App.UiRoot.ViewModel.Tabs.Remove(TargetTab);
        }

        // Refresh content to avoid layout conflicts
        TargetTab.RefreshContent();

        // Add to new window
        newWindow.CreateTab(TargetTab);

        // Show and activate new window
        newWindow.Show();
        newWindow.Activate();
    }

    public void MoveToWindow(IMirelTabWindow targetWindow)
    {
        if (TargetTab == null || targetWindow == null) return;
        if (SourceWindow == targetWindow) return; // Already in target window

        // Remove from source window
        if (SourceWindow is TabWindow sourceTabWindow)
        {
            sourceTabWindow.RemoveTab(TargetTab);
        }
        else if (App.UiRoot != null)
        {
            App.UiRoot.ViewModel.Tabs.Remove(TargetTab);
        }

        // Refresh content to avoid layout conflicts
        TargetTab.RefreshContent();

        // Add to target window
        if (targetWindow is TabWindow targetTabWindow)
        {
            targetTabWindow.CreateTab(TargetTab);
        }

        // Activate target window
        targetWindow.Activate();
    }

    public static IMirelTabWindow[] GetAllTabWindows()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime?.Windows == null) return Array.Empty<IMirelTabWindow>();

        return lifetime.Windows
            .OfType<IMirelTabWindow>()
            .ToArray();
    }
}