using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mirel.Classes.Entries;

public class PageInfoEntry : ReactiveObject
{
    [Reactive] public string Title { get; set; }
    [Reactive] public StreamGeometry Icon { get; init; }
    [Reactive] public bool CanClose { get; init; } = true;
    [Reactive] public bool ShowIcon { get; init; } = true;
}