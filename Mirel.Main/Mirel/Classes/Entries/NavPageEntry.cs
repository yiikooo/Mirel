using System;

namespace Mirel.Classes.Entries;

public class NavPageEntry(MirelStaticPageInfo staticPageInfo, Func<(object sender, object? args), object>? action)
{
    public readonly Func<(object sender, object? args), object>? Create = action;
    public MirelStaticPageInfo StaticPageInfo { get; set; } = staticPageInfo;
}