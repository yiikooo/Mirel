using System;
using Mirel.Classes.Interfaces;

namespace Mirel.Classes.Entries;

public class TabSEntry
{
    public required TabEntry Entry { get; init; }
    public required IMirelTabWindow Window { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is TabSEntry entry && entry.Entry == Entry && entry.Window == Window;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Entry, Window);
    }
}