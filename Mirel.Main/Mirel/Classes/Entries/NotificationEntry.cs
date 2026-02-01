using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Mirel.Const;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Classes.Entries;

public sealed class NotificationEntry(Notification Entry, NotificationType Type, DateTime Time, string Title = "Mirel")
    : ReactiveObject
{
    public Notification Entry { get; set; } = Entry;
    public NotificationType Type { get; } = Type;
    public DateTime Time { get; } = Time;
    public string Title { get; } = Title;
    [Reactive] public bool IsClosing { get; set; }

    public async void Remove()
    {
        if (IsClosing) return;
        IsClosing = true;
        await Task.Delay(300);
        UiProperty.Notifications.Remove(this);
    }


    public bool Equals(NotificationEntry? other)
    {
        return Entry == other?.Entry && Type == other.Type && Time == other.Time && Title == other.Title;
    }
}