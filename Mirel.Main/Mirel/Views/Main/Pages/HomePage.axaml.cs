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

public partial class HomePage : PageMixModelBase, IMirelTabPage
{
    public HomePage()
    {
        InitializeComponent();
        DataContext = this;
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        PageInfo = new PageInfoEntry
        {
            Title = "Mirel",
            CanClose = false
        };
    }

    public string ShortInfo { get; set; }
    public Control BottomElement { get; set; }
    public Control RootElement { get; set; }
    public PageLoadingAnimator InAnimator { get; set; }

    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }

    public void OnClose()
    {
    }
}