using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages;

public partial class GitManager : PageModelBase, IMirelNavPage
{
    public GitManager()
    {
        InitializeComponent();
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 0, 0, 0), (0.5, 1));
    }

    public Control RootElement { get; init; }
    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo => StaticPageInfo;
    public PageLoadingAnimator InAnimator { get; set; }

    public void OnClose()
    {
    }

    public static PageInfoEntry StaticPageInfo { get; } = new()
    {
        Title = "Git 管理",
        Icon = StreamGeometry.Parse(
            "F1 M448,512z M0,0z M439.6,236.1L244,40.5C238.6,35 231.2,32 223.6,32 216,32 208.6,35 203.2,40.4L162.5,81 214,132.5C241.1,123.4,266.7,149.3,257.4,176.2L307.1,225.9C341.3,214.1 368.3,256.9 342.6,282.6 316.1,309.1 272.4,279.7 286.6,245.3L240.3,199 240.3,320.9C265.6,333.4 262.6,362.7 249.4,375.9 243,382.3 234.2,386 225.1,386 216,386 207.3,382.4 200.8,375.9 183.2,358.3 189.7,329 212,319.9L212,196.9C191.2,188.4,187.4,166.2,193.4,151.9L142.6,101 8.5,235.1C3,240.6 0,247.9 0,255.5 0,263.1 3,270.5 8.5,275.9L204.1,471.6C209.5,477 216.8,480 224.5,480 232.2,480 239.5,477 244.9,471.6L439.6,276.9C445,271.5 448,264.1 448,256.5 448,248.9 445,241.5 439.6,236.1z")
    };

    public static IMirelPage Create(object sender, object? param = null)
    {
        return new GitManager();
    }
}