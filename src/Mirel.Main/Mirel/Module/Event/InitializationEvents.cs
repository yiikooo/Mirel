using System;
using Mirel.Views.Main.Components;

namespace Mirel.Module.Event;

public static class InitializationEvents
{
    public static event Action? BeforeReadSettings;

    internal static void RaiseBeforeReadSettings()
    {
        BeforeReadSettings?.Invoke();
    }

    public static event Action? BeforeUiLoaded;

    internal static void RaiseBeforeUiLoaded()
    {
        BeforeUiLoaded?.Invoke();
    }

    public static event Action? AfterUiLoaded;

    internal static void RaiseAfterUiLoaded()
    {
        AfterUiLoaded?.Invoke();
    }

    public static event Action<MoreButtonMenu>? MoreMenuLoaded;

    internal static void RaiseMoreMenuLoaded(MoreButtonMenu menu)
    {
        MoreMenuLoaded?.Invoke(menu);
    }
}