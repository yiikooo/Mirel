using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelNavPage
{
    static MirelStaticPageInfo StaticPageInfo { get; }
    static abstract IMirelNavPage Create((object sender, object? param)t);
}