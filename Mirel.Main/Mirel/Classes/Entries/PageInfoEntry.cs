using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Mirel.Classes.Entries;

public class PageInfoEntry : ReactiveObject
{
    [Reactive] public string Title { get; set; }
    [Reactive] public StreamGeometry Icon { get; init; }
    [Reactive] public bool CanClose { get; init; } = true;
}