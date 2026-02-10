using Avalonia.Media;

namespace Mirel.Classes.Entries;

public class MirelStaticPageInfo
{
    public StreamGeometry Icon { get; set; }
    public string Title { get; set; }
    public bool NeedPath { get; set; }
    public bool MustPath { get; set; }
    public bool AutoCreate { get; set; }
}