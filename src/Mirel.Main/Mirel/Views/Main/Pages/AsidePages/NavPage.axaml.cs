using System.Collections.ObjectModel;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Mirel.Views.Main.Components;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class NavPage : PageModelBase, IMirelPage
{
    private bool _fl = true;

    public NavPage()
    {
        InitializeComponent();
        RootElement = Root;
        DataContext = this;
        ListBox.SelectionChanged += (_, _) =>
        {
            if (ListBox.SelectedItem is not PageRegistration pageReg) return;
            var page = pageReg.CreateInstance(this, null);
            if (page is not IMirelTabPage tabPage) return;

            if (tabPage is IMirelSingletonTabPage)
            {
                SingletonTabManager.CreateOrActivateSingletonTab(tabPage);
            }
            else
            {
                var tab = new TabEntry(tabPage);
                if (UiProperty.ActiveWindow is TabWindow tabWindow)
                {
                    tabWindow.CreateTab(tab);
                }
                else
                {
                    App.UiRoot.CreateTab(tab);
                }
            }

            ListBox.SelectedItem = null;
        };
        Loaded += (_, _) =>
        {
            if (!_fl) return;
            _fl = false;
            LoadPages();
        };
    }

    public ObservableCollection<PageRegistration> Pages { get; set; } = new();

    public Control RootElement { get; init; }

    private void LoadPages()
    {
        Pages.Clear();
        var allPages = PageNav.GetAllPages();
        foreach (var page in allPages)
        {
            Pages.Add(page);
        }
    }
}