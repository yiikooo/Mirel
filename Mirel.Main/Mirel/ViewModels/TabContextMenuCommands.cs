using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Service;
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

        // Use the MoveTabToNewWindow extension method from TabDragDropService
        TargetTab.MoveTabToNewWindow();
    }

    public void MoveToWindow(IMirelTabWindow targetWindow)
    {
        if (TargetTab == null || targetWindow == null) return;
        if (SourceWindow == targetWindow) return; // Already in target window

        // Use the MoveTabToWindow extension method from TabDragDropService
        if (targetWindow is Window window)
        {
            TargetTab.MoveTabToWindow(window);
        }
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