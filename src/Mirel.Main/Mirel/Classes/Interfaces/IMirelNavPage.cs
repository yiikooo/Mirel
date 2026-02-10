using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelNavPage
{
    public static abstract MirelStaticPageInfo StaticPageInfo { get; }
    public static abstract IMirelPage Create(object sender, object? param = null);
}