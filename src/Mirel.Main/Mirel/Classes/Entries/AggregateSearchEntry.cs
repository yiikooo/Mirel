using Avalonia.Media;
using Mirel.Classes.Enums;

namespace Mirel.Classes.Entries;

public record AggregateSearchEntry
{
    public AggregateSearchType Type { get; init; }
    public StreamGeometry Icon { get; init; }
    public string Title { get; init; }
    public string Label { get; init; }
    public object Data { get; init; }
}