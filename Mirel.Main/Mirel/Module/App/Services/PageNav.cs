using Mirel.Classes.Entries;
using Mirel.Const;
using Mirel.Views.Main.Pages;

namespace Mirel.Module.App.Services;

public class PageNav
{
    public static void Main()
    {
        UiProperty.NavPages.Add(new NavPageEntry(NewTabPage.StaticPageInfo, NewTabPage.Create));
    }
}