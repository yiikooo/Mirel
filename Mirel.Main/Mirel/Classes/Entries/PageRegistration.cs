using System;
using System.Reflection;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;

namespace Mirel.Classes.Entries;

public class PageRegistration
{
    public PageRegistration(PageIdentifier identifier, Type pageType, MirelStaticPageInfo staticPageInfo)
    {
        Identifier = identifier;
        PageType = pageType;
        StaticPageInfo = staticPageInfo;
    }

    public PageIdentifier Identifier { get; set; }
    public Type PageType { get; set; }
    public MirelStaticPageInfo StaticPageInfo { get; set; }

    public IMirelPage CreateInstance(object sender, object? param = null)
    {
        var method = PageType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        return (IMirelPage)method.Invoke(null, new[] { sender, param });
    }
}