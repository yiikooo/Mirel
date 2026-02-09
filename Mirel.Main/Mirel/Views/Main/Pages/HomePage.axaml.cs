using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages;

public partial class HomePage : PageModelBase, IMirelTabPage, IMirelNavPage
{
    public HomePage()
    {
        InitializeComponent();
        DataContext = this;
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        PageInfo = new PageInfoEntry
        {
            Title = "Mirel", ShowIcon = false,
            CanClose = false,
            Icon = StreamGeometry.Parse(
                "F1 M512,512z M0,0z M234.2,8.6C246.5,-2.8,265.5,-2.8,277.7,8.6L368,92.3 368,80C368,62.3,382.3,48,400,48L432,48C449.7,48,464,62.3,464,80L464,181.5 501.8,216.6C511.4,225.6 514.6,239.5 509.8,251.7 505,263.9 493.2,272 480,272L464,272 464,448C464,483.3,435.3,512,400,512L112,512C76.7,512,48,483.3,48,448L48,272 32,272C18.8,272 7,263.9 2.2,251.7 -2.6,239.5 0.599999999999999,225.5 10.2,216.6L234.2,8.59999999999999z M240,320C213.5,320,192,341.5,192,368L192,464 320,464 320,368C320,341.5,298.5,320,272,320L240,320z")
        };
    }

    public string ShortInfo { get; set; }

    public static IMirelPage Create(object sender, object? param = null)
    {
        return new HomePage();
    }

    public Control RootElement { get; init; }
    public PageLoadingAnimator InAnimator { get; set; }

    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }

    public void OnClose()
    {
    }
}