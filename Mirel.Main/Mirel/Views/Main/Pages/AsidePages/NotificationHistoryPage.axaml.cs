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

public partial class NotificationHistoryPage : PageMixModelBase, IMirelPage
{
    public static ObservableCollection<NotificationEntry> Notifications => UiProperty.HistoryNotifications;

    public NotificationHistoryPage()
    {
        InitializeComponent();
        RootElement = Root;
        DataContext = this;
    }

    public Control RootElement { get; init; }
}