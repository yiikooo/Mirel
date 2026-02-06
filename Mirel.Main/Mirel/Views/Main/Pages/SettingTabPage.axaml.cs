using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Ui;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Ursa.Controls;

namespace Mirel.Views.Main.Pages;

public partial class SettingTabPage: PageModelBase, IMirelTabPage, IMirelNavPage
{
    private SelectionListItem _selectedItem;
    public int DefaultNav = 0;

    public SettingTabPage()
    {
        InitializeComponent();
        DataContext = this;
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        PageInfo = new PageInfoEntry
        {
            Icon = Icons.Setting,
            Title = "设置"
        };
    }
    

    public string ShortInfo
    {
        get;
        set => SetField(ref field, value);
    } = "";

    public Control RootElement { get; init; }
    public PageLoadingAnimator InAnimator { get; set; }

    public SelectionListItem? SelectedItem
    {
        get => _selectedItem;
        set => SetField(ref _selectedItem, value);
    }

    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }
    public void OnClose()
    {
    }

    public static IMirelNavPage Create((object sender, object? param) t)
    {
        var root = ((Control)t.sender).GetVisualRoot();
        if (root is TabWindow tabWindow)
        {
            tabWindow.TogglePage("setting", new SettingTabPage());
            return null;
        }
        App.UiRoot.TogglePage("setting", new SettingTabPage());
        return null;    }
}