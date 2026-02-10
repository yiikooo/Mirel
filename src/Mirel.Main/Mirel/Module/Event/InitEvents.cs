using System;
using MoreButtonMenu = Mirel.Views.Main.Components.MoreButtonMenu;

namespace Mirel.Module.Event;

public class InitEvents
{
    public static event EventHandler? BeforeReadSettings;

    internal static void OnBeforeReadSettings()
    {
        BeforeReadSettings?.Invoke(null, EventArgs.Empty);
    }

    public static event EventHandler? BeforeUiLoaded;

    internal static void OnBeforeUiLoaded()
    {
        BeforeUiLoaded?.Invoke(null, EventArgs.Empty);
    }

    public static event EventHandler? AfterUiLoaded;

    internal static void OnAfterUiLoaded()
    {
        AfterUiLoaded?.Invoke(null, EventArgs.Empty);
    }

    public static event AppEventsHandler.OnlySenderHandler? MoreMenuLoaded;

    internal static void OnMoreMenuLoaded(MoreButtonMenu menu)
    {
        MoreMenuLoaded?.Invoke(menu);
    }
}