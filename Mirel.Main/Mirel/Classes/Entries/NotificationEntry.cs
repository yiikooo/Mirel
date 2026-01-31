using System;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public record NotificationEntry(Notification Entry, NotificationType Type, DateTime Time, string Title = "Mirel")
{
    public Notification Entry { get; set; } = Entry;
    public NotificationType Type { get; } = Type;
    public DateTime Time { get; } = Time;
    public string Title { get; } = Title;

    public void Remove()
    {
        UiProperty.Notifications.Remove(this);
    }


    public virtual bool Equals(NotificationEntry? other)
    {
        return Entry == other?.Entry && Type == other.Type && Time == other.Time && Title == other.Title;
    }
}