using System.Collections.Generic;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Module;
using Mirel.Views.Main.Pages;

namespace Mirel.Views.Main;

public static class PageNav
{
    public static void Initialize()
    {
        PageManager.RegisterPage<SettingTabPage>(PageIdentifier.Settings, SettingTabPage.StaticPageInfo);
        PageManager.RegisterPage<NewTabPage>(PageIdentifier.NewTab, NewTabPage.StaticPageInfo);
        PageManager.RegisterPage<HomePage>(PageIdentifier.Home, HomePage.StaticPageInfo);
        PageManager.RegisterPage<DebugPage>(PageIdentifier.Debug, DebugPage.StaticPageInfo);
    }

    public static IMirelPage? GetPage(PageIdentifier identifier, object sender, object? param = null)
    {
        return PageManager.GetPageInstance(identifier, sender, param);
    }

    public static MirelStaticPageInfo? GetPageInfo(PageIdentifier identifier)
    {
        return PageManager.GetPageRegistration(identifier)?.StaticPageInfo;
    }

    public static IReadOnlyList<PageRegistration> GetAllPages()
    {
        return PageManager.GetAllRegistrations();
    }
}