using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Mirel.Classes.Entries;

namespace Mirel.Module.Event;

public static class ApplicationEvents
{
    public static event Action? SaveSettings;

    internal static void RaiseSaveSettings()
    {
        SaveSettings?.Invoke();
    }


    public static event Func<Task<bool>>? AppExiting;

    internal static async Task<bool> RaiseAppExiting()
    {
        if (AppExiting == null) return true;

        foreach (var handler in AppExiting.GetInvocationList())
        {
            var asyncHandler = (Func<Task<bool>>)handler;
            var canContinue = await asyncHandler.Invoke();

            if (!canContinue)
            {
                return false;
            }
        }

        return true;
    }

    public static event Action<object?, DragEventArgs>? AppDragDrop;

    internal static void RaiseAppDragDrop(object? sender, DragEventArgs e)
    {
        AppDragDrop?.Invoke(sender, e);
    }

    public static event Action<TabSEntry>? TabSelectionChanged;


    internal static void RaiseTabSelectionChanged(TabSEntry tab)
    {
        TabSelectionChanged?.Invoke(tab);
    }
}