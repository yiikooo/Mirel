using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class NotificationPage : PageMixModelBase, IMirelPage
{
    public static ObservableCollection<NotificationEntry> Notifications => UiProperty.Notifications;

    public NotificationPage()
    {
        InitializeComponent();
        RootElement = Root;
        DataContext = this;
    }

    public string ShortInfo { get; set; }
    public Control RootElement { get; init; }
    public PageLoadingAnimator InAnimator { get; set; }
}