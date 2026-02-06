using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Mirel.Classes.Entries;
using Ursa.Controls;

namespace Mirel.Controls;

[PseudoClasses(PC_Information, PC_Success, PC_Warning, PC_Error)]
public abstract class MirelMessageCard : ContentControl
{
    public const string PC_Information = ":information";
    public const string PC_Success = ":success";
    public const string PC_Warning = ":warning";
    public const string PC_Error = ":error";

    private bool _isClosing;

    static MirelMessageCard()
    {
        CloseOnClickProperty.Changed.AddClassHandler<Button>(OnCloseOnClickPropertyChanged);
    }

    public MirelMessageCard()
    {
        UpdateNotificationType();
    }

    public bool IsClosing
    {
        get => _isClosing;
        private set => SetAndRaise(IsClosingProperty, ref _isClosing, value);
    }

    public static readonly DirectProperty<MirelMessageCard, bool> IsClosingProperty =
        AvaloniaProperty.RegisterDirect<MirelMessageCard, bool>(nameof(IsClosing), o => o.IsClosing);

    public bool IsClosed
    {
        get => GetValue(IsClosedProperty);
        set => SetValue(IsClosedProperty, value);
    }

    public static readonly StyledProperty<bool> IsClosedProperty =
        AvaloniaProperty.Register<MirelMessageCard, bool>(nameof(IsClosed));

    public NotificationType NotificationType
    {
        get => GetValue(NotificationTypeProperty);
        set => SetValue(NotificationTypeProperty, value);
    }

    public static readonly StyledProperty<NotificationType> NotificationTypeProperty =
        AvaloniaProperty.Register<MirelMessageCard, NotificationType>(nameof(NotificationType));

    public bool ShowIcon
    {
        get => GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    public static readonly StyledProperty<bool> ShowIconProperty =
        AvaloniaProperty.Register<MirelMessageCard, bool>(nameof(ShowIcon), true);

    public bool ShowClose
    {
        get => GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    public static readonly StyledProperty<bool> ShowCloseProperty =
        AvaloniaProperty.Register<MirelMessageCard, bool>(nameof(ShowClose), true);

    public IList<OperateButtonEntry>? OperateButtons
    {
        get => GetValue(OperateButtonsProperty);
        set => SetValue(OperateButtonsProperty, value);
    }

    public static readonly StyledProperty<IList<OperateButtonEntry>?> OperateButtonsProperty =
        AvaloniaProperty.Register<MirelMessageCard, IList<OperateButtonEntry>?>(nameof(OperateButtons));

    public bool IsButtonsInline
    {
        get => GetValue(IsButtonsInlineProperty);
        set => SetValue(IsButtonsInlineProperty, value);
    }

    public static readonly StyledProperty<bool> IsButtonsInlineProperty =
        AvaloniaProperty.Register<MirelMessageCard, bool>(nameof(IsButtonsInline), false);

    public NotificationEntry? NotificationEntry
    {
        get => GetValue(NotificationEntryProperty);
        set => SetValue(NotificationEntryProperty, value);
    }

    public static readonly StyledProperty<NotificationEntry?> NotificationEntryProperty =
        AvaloniaProperty.Register<MirelMessageCard, NotificationEntry?>(nameof(NotificationEntry));

    public static readonly RoutedEvent<RoutedEventArgs> MessageClosedEvent =
        RoutedEvent.Register<MirelMessageCard, RoutedEventArgs>(nameof(MessageClosed), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? MessageClosed
    {
        add => AddHandler(MessageClosedEvent, value);
        remove => RemoveHandler(MessageClosedEvent, value);
    }

    public static bool GetCloseOnClick(Button obj)
    {
        _ = obj ?? throw new ArgumentNullException(nameof(obj));
        return obj.GetValue(CloseOnClickProperty);
    }

    public static void SetCloseOnClick(Button obj, bool value)
    {
        _ = obj ?? throw new ArgumentNullException(nameof(obj));
        obj.SetValue(CloseOnClickProperty, value);
    }

    public static readonly AttachedProperty<bool> CloseOnClickProperty =
        AvaloniaProperty.RegisterAttached<MirelMessageCard, Button, bool>("CloseOnClick", defaultValue: false);

    private static void OnCloseOnClickPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
    {
        var button = (Button)d;
        var value = (bool)e.NewValue!;
        if (value)
        {
            button.Click += Button_Click;
        }
        else
        {
            button.Click -= Button_Click;
        }
    }

    private static void Button_Click(object? sender, RoutedEventArgs e)
    {
        var btn = sender as ILogical;
        var message = btn?.GetLogicalAncestors().OfType<MirelMessageCard>().FirstOrDefault();
        message?.Close();
    }

    public void Close()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;
        IsClosed = true;

        NotificationEntry?.Remove();
    }

    /// <summary>
    /// 关闭 Toast 卡片但不从通知列表中移除
    /// </summary>
    public void CloseWithoutRemovingFromList()
    {
        if (IsClosing)
        {
            return;
        }

        IsClosing = true;
        IsClosed = true;
        // 不调用 NotificationEntry?.Remove()
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == ContentProperty && e.NewValue is IMessage message)
        {
            SetValue(NotificationTypeProperty, message.Type);
        }

        if (e.Property == NotificationTypeProperty)
        {
            UpdateNotificationType();
        }

        if (e.Property == IsClosedProperty)
        {
            if (!IsClosing && !IsClosed)
            {
                return;
            }

            RaiseEvent(new RoutedEventArgs(MessageClosedEvent));
        }
    }

    private void UpdateNotificationType()
    {
        switch (NotificationType)
        {
            case NotificationType.Error:
                PseudoClasses.Add(PC_Error);
                break;

            case NotificationType.Information:
                PseudoClasses.Add(PC_Information);
                break;

            case NotificationType.Success:
                PseudoClasses.Add(PC_Success);
                break;

            case NotificationType.Warning:
                PseudoClasses.Add(PC_Warning);
                break;
        }
    }
}