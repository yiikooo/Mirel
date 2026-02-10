using System.Collections.ObjectModel;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class NotificationPage : PageModelBase, IMirelPage
{
    public NotificationPage()
    {
        InitializeComponent();
        RootElement = Root;
        DataContext = this;
    }

    public static ObservableCollection<NotificationEntry> Notifications => UiProperty.Notifications;

    public string ShortInfo { get; set; }
    public PageLoadingAnimator InAnimator { get; set; }
    public Control RootElement { get; init; }
}