using Avalonia;
using Mirel.Classes.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mirel.Classes.Entries;

public class NavEntry(string title, IMirelPage page, string icon) : ReactiveObject
{
    [Reactive] public string Title { get; set; } = title;
    [Reactive] public string Icon { get; set; } = icon;
    [Reactive] public IMirelPage Page { get; set; } = page;
    [Reactive] public double IconWidth { get; set; } = 14;
    [Reactive] public double IconHeight { get; set; } = 14;
    [Reactive] public bool Dot { get; set; }
    [Reactive] public string Tag { get; set; }
    [Reactive] public Thickness IconMargin { get; set; } = new(0);
}