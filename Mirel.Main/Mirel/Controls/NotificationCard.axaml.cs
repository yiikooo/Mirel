using Avalonia;
using Avalonia.Controls;
using Mirel.Classes.Entries;

namespace Mirel.Controls;

public partial class NotificationCard : UserControl
{
    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsCloseButtonVisible), true);

    public static readonly StyledProperty<NotificationEntry?> NotificationEntryProperty =
        AvaloniaProperty.Register<NotificationCard, NotificationEntry?>(nameof(NotificationEntry));

    public NotificationCard()
    {
        InitializeComponent();
    }

    public bool IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    public NotificationEntry? NotificationEntry
    {
        get => GetValue(NotificationEntryProperty);
        set => SetValue(NotificationEntryProperty, value);
    }
}