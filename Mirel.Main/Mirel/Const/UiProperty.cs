using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Mirel.Classes.Entries;
using ReactiveUI;
using Ursa.Controls;
using WindowNotificationManager = Avalonia.Controls.Notifications.WindowNotificationManager;

namespace Mirel.Const;

public class UiProperty : ReactiveObject
{
    private static UiProperty? _instance;

    static UiProperty()
    {
    }

    public static UiProperty Instance
    {
        get { return _instance ??= new UiProperty(); }
    }

    public static ObservableCollection<NotificationEntry> Notifications { get; } = [];
    public static ObservableCollection<string> BuiltInTags { get; } = [];
    // public static WindowNotificationManager Notification => ActiveWindow.Notification;
    // public static WindowToastManager Toast => ActiveWindow.Toast;

    // public static IAurelioWindow ActiveWindow => (Application.Current!.ApplicationLifetime as
    //     IClassicDesktopStyleApplicationLifetime).Windows.FirstOrDefault
    //     (x => x.IsActive) as IAurelioWindow ?? App.UiRoot;
    //
    // public static ObservableCollection<NavPageEntry> NavPages { get; } = [];
}