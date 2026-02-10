using System.Collections.ObjectModel;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages.AsidePages;

public partial class NotificationHistoryPage : PageModelBase, IMirelPage
{
    public NotificationHistoryPage()
    {
        InitializeComponent();
        RootElement = Root;
        DataContext = this;
    }

    public static ObservableCollection<NotificationEntry> Notifications => UiProperty.HistoryNotifications;

    public Control RootElement { get; init; }
}