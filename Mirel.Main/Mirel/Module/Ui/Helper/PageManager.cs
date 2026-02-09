using System;
using System.Collections.Generic;
using System.Linq;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;

namespace Mirel.Module.Ui.Helper;

public static class PageManager
{
    private static readonly List<PageRegistration> RegisteredPages = new();

    public static void RegisterPage(PageIdentifier identifier, Type pageType, MirelStaticPageInfo staticPageInfo)
    {
        if (pageType == null)
            throw new ArgumentNullException(nameof(pageType));

        if (!typeof(IMirelNavPage).IsAssignableFrom(pageType))
            throw new ArgumentException($"页面类型必须实现 {nameof(IMirelNavPage)} 接口", nameof(pageType));

        if (RegisteredPages.Any(p => p.Identifier == identifier))
            throw new InvalidOperationException($"标识符 '{identifier}' 已被注册");

        var registration = new PageRegistration(identifier, pageType, staticPageInfo);
        RegisteredPages.Add(registration);
    }

    public static void RegisterPage<TPage>(PageIdentifier identifier, MirelStaticPageInfo staticPageInfo)
        where TPage : IMirelNavPage
    {
        RegisterPage(identifier, typeof(TPage), staticPageInfo);
    }

    public static IMirelPage? GetPageInstance(PageIdentifier identifier, object sender, object? param = null)
    {
        var registration = RegisteredPages.FirstOrDefault(p => p.Identifier == identifier);
        return registration?.CreateInstance(sender, param);
    }

    public static PageRegistration? GetPageRegistration(PageIdentifier identifier)
    {
        return RegisteredPages.FirstOrDefault(p => p.Identifier == identifier);
    }

    public static IReadOnlyList<PageRegistration> GetAllRegistrations()
    {
        return RegisteredPages.AsReadOnly();
    }

    public static bool IsRegistered(PageIdentifier identifier)
    {
        return RegisteredPages.Any(p => p.Identifier == identifier);
    }

    public static bool UnregisterPage(PageIdentifier identifier)
    {
        var registration = RegisteredPages.FirstOrDefault(p => p.Identifier == identifier);
        if (registration != null)
        {
            RegisteredPages.Remove(registration);
            return true;
        }

        return false;
    }

    public static void ClearAll()
    {
        RegisteredPages.Clear();
    }
}