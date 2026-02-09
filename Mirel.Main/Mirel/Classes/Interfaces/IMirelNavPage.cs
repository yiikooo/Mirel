using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelNavPage
{
    static MirelStaticPageInfo StaticPageInfo { get; }
    static abstract IMirelPage Create(object sender, object? param = null);
}