using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public record NotificationEntry(
    Notification entry,
    NotificationType type,
    DateTime time,
    IList<OperateButtonEntry>? operateButtons = null)
{
    public Notification Entry { get; set; } = entry;
    public NotificationType Type { get; } = type;
    public DateTime Time { get; } = time;
    public IList<OperateButtonEntry>? OperateButtons { get; set; } = operateButtons;
    public NotificationEntry Self => this;

    public void Remove()
    {
        UiProperty.Notifications.Remove(this);
    }
}