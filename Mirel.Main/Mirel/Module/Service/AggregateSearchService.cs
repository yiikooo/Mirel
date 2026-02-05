using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;

namespace Mirel.Module.Service;

public class AggregateSearchService
{
    private const StringComparison _searchComparison = StringComparison.OrdinalIgnoreCase;

    public static void HandleSelection(AggregateSearchEntry entry, Control sender)
    {
        if(entry.Type == AggregateSearchType.All) return;

        if (entry.Type == AggregateSearchType.Tab)
        {
            if (entry.Data is not TabEntry tab) return;
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime?.Windows == null) return;
            var windows = lifetime.Windows
                .OfType<IMirelTabWindow>()
                .ToList();
            foreach (var window in windows.Where(window => window.Tabs.Contains(tab)))
            {
                window.SelectTab(tab);
                window.Activate();
                break;
            }
        }
    }

    public static List<AggregateSearchEntry> Search(string query, AggregateSearchType type = AggregateSearchType.All,
        List<AggregateSearchEntry>? items = null)
    {
        items ??= GetAggregateItems();

        if (string.IsNullOrEmpty(query))
        {
            return items;
        }

        var filtered = items.AsEnumerable();

        if (type != AggregateSearchType.All)
        {
            filtered = filtered.Where(e => e.Type == type);
        }

        filtered = filtered.Where(e => e.Title != null && e.Title.Contains(query, _searchComparison));

        return filtered.ToList();
    }

    public static List<AggregateSearchEntry> GetAggregateItems(bool order = false)
    {
        List<AggregateSearchEntry> items = [];
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        if (lifetime?.Windows == null)
        {
            return order ? items.OrderBy(e => e.Type).ThenBy(e => e.Title).ToList() : items;
        }

        var windows = lifetime.Windows
            .OfType<IMirelTabWindow>()
            .ToList();

        foreach (var window in windows)
        {
            if (window.Tabs == null) continue;

            items.AddRange(window.Tabs.Select(tab => new AggregateSearchEntry
            {
                Title = tab.Title, Icon = tab.Icon, Type = AggregateSearchType.Tab, Label = "标签页",
                Data = tab
            }));
        }

        if (order)
        {
            items = items.OrderBy(e => e.Type).ThenBy(e => e.Title).ToList();
        }

        return items;
    }
}