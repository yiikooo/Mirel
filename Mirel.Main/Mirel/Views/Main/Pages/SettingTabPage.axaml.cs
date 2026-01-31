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

public partial class SettingTabPage: PageMixModelBase, IMirelTabPage, IMirelNavPage
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
    
    private string _shortInfo = string.Empty;

    public string ShortInfo
    {
        get => _shortInfo;
        set => SetField(ref _shortInfo, value);
    }

    public Control BottomElement { get; set; }
    public Control RootElement { get; set; }
    public PageLoadingAnimator InAnimator { get; set; }

    public SelectionListItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetField(ref _selectedItem, value);
            ShortInfo = value == null ? "设置" : (value.Tag as IMirelPage).ShortInfo;
        }
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