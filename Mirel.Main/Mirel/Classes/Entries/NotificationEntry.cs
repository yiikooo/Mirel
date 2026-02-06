using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public sealed class NotificationEntry : ReactiveObject
{
    public Notification Entry { get; set; }
    public NotificationType Type { get; }
    public DateTime Time { get; }
    public string Title { get; }
    [Reactive] public bool IsClosing { get; set; }
    public IList<OperateButtonEntry>? OperateButtons { get; set; }

    public NotificationEntry(Notification entry, NotificationType type, DateTime time, string title = "Mirel", IList<OperateButtonEntry>? operateButtons = null)
    {
        Entry = entry;
        Type = type;
        Time = time;
        Title = title;
        OperateButtons = operateButtons;
    }

    public void Remove()
    {
        UiProperty.Notifications.Remove(this);
    }

    public void RemoveAndDelete()
    {
        UiProperty.Notifications.Remove(this);
    }

    public bool Equals(NotificationEntry? other)
    {
        return Entry == other?.Entry && Type == other.Type && Time == other.Time && Title == other.Title;
    }
}