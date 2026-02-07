using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Mirel.Classes.Entries;

namespace Mirel.Module.Events;

public class AppEvents
{
    public static event EventHandler? SaveSettings;

    internal static void OnSaveSettings()
    {
        SaveSettings?.Invoke(null, EventArgs.Empty);
    }

    public static event AppEventsHandler.AppExitingHandler? AppExiting;

    internal static async Task<bool> OnAppExiting()
    {
        var canExit = true;

        if (AppExiting == null) return canExit;
        foreach (var @delegate in AppExiting.GetInvocationList())
        {
            var handler = (AppEventsHandler.AppExitingHandler)@delegate;
            if (await handler.Invoke()) continue;
            canExit = false;
            break;
        }

        return canExit;
    }

    public static event AppEventsHandler.AppDragDropHandler? AppDragDrop;

    internal static void OnAppDragDrop(object? sender, DragEventArgs e)
    {
        AppDragDrop?.Invoke(sender, e);
    }

    public static event AppEventsHandler.TabSelectionChangedHandler? TabSelectionChanged;

    internal static void OnTabSelectionChanged(TabSEntry e)
    {
        TabSelectionChanged?.Invoke(e);
    }
}