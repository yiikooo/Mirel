using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelNavPage : IMirelTabPage
{
    public static abstract PageInfoEntry StaticPageInfo { get; }
    public static abstract IMirelPage Create(object sender, object? param = null);
}