using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DynamicData;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;

namespace Mirel.Module.Service;

public class TabService
{
    public static ObservableCollection<TabSEntry> AppTabs { get; } = [];
    private static readonly Func<IMirelTabWindow, bool> _windowFilter = w => w.Tabs != null;

    private static readonly Func<IMirelTabWindow, TabEntry, TabSEntry> _entryFactory = (w, t) =>
        new TabSEntry { Entry = t, Window = w };

    private static TabService? _instance;

    static TabService()
    {
    }

    public static TabService Instance
    {
        get { return _instance ??= new TabService(); }
    }

    public static void UpdateTabs()
    {
        AppTabs.Clear();
        AppTabs.AddRange(GetTabs());
    }

    public static List<TabSEntry> GetTabs()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime?.Windows is null) return [];

        return lifetime.Windows
            .OfType<IMirelTabWindow>()
            .Where(_windowFilter)
            .SelectMany(w => w.Tabs.Select(t => _entryFactory(w, t)))
            .ToList();
    }
}