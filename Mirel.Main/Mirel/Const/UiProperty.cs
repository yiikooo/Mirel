using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Ui.Helper;
using ReactiveUI;
using Ursa.Controls;

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

    public static ThemeVariant Mirage { get; } = new("Mirage", ThemeVariant.Dark);
    public static ObservableCollection<NotificationEntry> Notifications { get; } = [];
    public static WindowNotificationManager Notification => ActiveWindow.Notification;
    public static MirelWindowToastManager Toast => ActiveWindow.Toast;

    public static IMirelWindow ActiveWindow => (Application.Current!.ApplicationLifetime as
        IClassicDesktopStyleApplicationLifetime).Windows.FirstOrDefault
        (x => x.IsActive) as IMirelWindow ?? App.UiRoot;

    public static ObservableCollection<NavPageEntry> NavPages { get; } = [];
}