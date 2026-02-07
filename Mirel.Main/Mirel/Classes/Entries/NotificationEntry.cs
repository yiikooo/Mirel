using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public class NotificationEntry : ReactiveObject
{
    public NotificationEntry(Notification Entry,
        NotificationType Type,
        DateTime Time,
        ObservableCollection<OperateButtonEntry>? OperateButtons = null)
    {
        this.Entry = Entry;
        this.Type = Type;
        this.Time = Time;
        this.OperateButtons = OperateButtons;
    }

    public ObservableCollection<OperateButtonEntry>? OperateButtons { get; set; }
    public NotificationEntry Hi => this;
    [Reactive] public Notification Entry { get; init; }
    [Reactive] public NotificationType Type { get; init; }
    [Reactive] public DateTime Time { get; init; }

    public void Remove()
    {
        UiProperty.Notifications.Remove(this);
    }
}