using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public sealed class NotificationEntry(
    Notification entry,
    NotificationType type,
    DateTime time,
    string title = "Mirel",
    IList<OperateButtonEntry>? operateButtons = null,
    bool isCloseButtonVisible = true)
    : ReactiveObject
{
    public Notification Entry { get; set; } = entry;
    public NotificationType Type { get; } = type;
    public DateTime Time { get; } = time;
    public string Title { get; } = title;
    public IList<OperateButtonEntry>? OperateButtons { get; set; } = operateButtons;

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