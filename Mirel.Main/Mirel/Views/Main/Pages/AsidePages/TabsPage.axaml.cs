using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.Core;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Events;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class TabsPage : PageMixModelBase, IMirelPage
{
    public TabsPage()
    {
        InitializeComponent();
        RootElement = Root;
        ListBox.SelectionChanged += (_, _) =>
        {
            if (ListBox.SelectedItem is not TabSEntry tab) return;
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime?.Windows == null) return;
            var windows = lifetime.Windows
                .OfType<IMirelTabWindow>()
                .ToList();
            foreach (var window in windows.Where(window => window.Tabs.Contains(tab.Entry)))
            {
                window.SelectTab(tab.Entry);
                window.Activate();
                break;
            }
        };
        AppEvents.TabSelectionChanged += e =>
        {
            ListBox.SelectedItem = e;
        };
    }

    public Control RootElement { get; init; }
}