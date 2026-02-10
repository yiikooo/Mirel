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
    public TabEntry TargetTab
    {
        get;
        set => SetField(ref field, value);
    }

    public IMirelTabWindow SourceWindow
    {
        get;
        set => SetField(ref field, value);
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
        if (lifetime?.Windows == null) return [];

        return lifetime.Windows
            .OfType<IMirelTabWindow>()
            .ToArray();
    }
}