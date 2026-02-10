using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Interfaces;
using Mirel.Module.Event;
using Mirel.Views.Main;

namespace Mirel.Module.App.Services;

public class AppExit
{
    public static void Main()
    {
        ApplicationEvents.AppExiting += AppEventsOnAppExiting;
    }

    private static async Task<bool> AppEventsOnAppExiting()
    {
        var requestCloseTab = await RequestCloseTabs();
        return requestCloseTab;
    }

    private static async Task<bool> RequestCloseTabs()
    {
        foreach (var tabEntry in Mirel.App.UiRoot.Tabs
                     .Where(x => x.Content is IMirelRequestableClosePage))
        {
            var close = await (tabEntry.Content as IMirelRequestableClosePage).RequestClose(Mirel.App.UiRoot);
            if (!close) return false;
        }

        var ws = (Application.Current!.ApplicationLifetime as
            IClassicDesktopStyleApplicationLifetime).Windows.OfType<TabWindow>();
        foreach (var tabWindow in ws)
        foreach (var tabEntry in tabWindow.Tabs
                     .Where(x => x.Content is IMirelRequestableClosePage))
        {
            var close = await (tabEntry.Content as IMirelRequestableClosePage).RequestClose(Mirel.App.UiRoot);
            if (!close) return false;
        }

        return true;
    }
}