using Avalonia;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.ViewModels;
using Ursa.Controls;

namespace Mirel.Controls;

public partial class NotificationCard : PageMixModelBase
{
    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsCloseButtonVisible), defaultValue: true);

    public bool IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }
    
    public static readonly StyledProperty<NotificationEntry> NotificationEntryProperty =
        AvaloniaProperty.Register<NotificationCard, NotificationEntry>(nameof(NotificationEntry));

    public NotificationEntry NotificationEntry
    {
        get => GetValue(NotificationEntryProperty);
        set => SetValue(NotificationEntryProperty, value);
    }
    
    public NotificationCard()
    {
        DataContext = this;
        InitializeComponent();
    }
}