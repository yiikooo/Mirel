using Avalonia;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Ui;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages;

public partial class TaskingPage : PageModelBase, IMirelNavPage
{
    public TaskingPage()
    {
        InitializeComponent();
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        DataContext = Tasking.Instance;
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
        Title = "任务管理",
        Icon = Icons.Model3D
    };

    public static IMirelPage Create(object sender, object? param = null)
    {
        return new TaskingPage();
    }
}