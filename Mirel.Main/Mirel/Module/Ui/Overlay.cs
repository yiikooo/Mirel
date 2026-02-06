using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Views.Main;
using Ursa.Controls;
using Notification = Ursa.Controls.Notification;

namespace Mirel.Module.Ui;

public abstract class Overlay
{
    public static async Task<ContentDialogResult> ShowDialogAsync(string title = "Title", string msg = null,
        Control? p_content = null, string b_primary = null, string b_cancel = null, string b_secondary = null,
        TopLevel? p_host = null, Control? sender = null)
    {
        var content = p_content ?? new SelectableTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Text = msg
        };
        if (!string.IsNullOrWhiteSpace(msg) && p_content != null)
            content = new StackPanel
            {
                Spacing = 15,
                Children =
                {
                    new SelectableTextBlock
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Text = msg
                    },
                    content
                }
            };

        if (string.IsNullOrWhiteSpace(msg) && p_content == null) content = null;

        var dialog = new ContentDialog
        {
            PrimaryButtonText = b_primary,
            Content = content,
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = b_cancel,
            SecondaryButtonText = b_secondary,
            Title = title
        };
        var result = await dialog.ShowAsync(p_host ?? TopLevel.GetTopLevel(sender ?? Mirel.App.UiRoot));
        return result;
    }

    public static void Notice(string msg, NotificationType type = NotificationType.Information, TimeSpan? time = null,
        Action? clickAction = null, Action? closeAction = null, IMirelWindow? host = null,
        IList<OperateButtonEntry>? operateButtons = null, bool isButtonsInline = false)
    {
        var t = DateTime.Now;
        Logger.Info($"[Notice] [{type}] {msg}");
        var notification = new Notification("Mirel", msg, type);
        var entry = new NotificationEntry(notification, notification.Type, t, operateButtons: operateButtons);
        UiProperty.Notifications.Insert(0, entry);
        UiProperty.HistoryNotifications.Insert(0, entry);

        NotificationBubble(msg, type, entry, closeAction, time, clickAction, host, operateButtons, isButtonsInline);
    }

    public static void NotificationBubble(string msg, NotificationType type, NotificationEntry entry,
        Action? closeAction = null, TimeSpan? time = null,
        Action? clickAction = null, IMirelWindow? host = null, IList<OperateButtonEntry>? operateButtons = null,
        bool isButtonsInline = false)
    {
        var toast = new Toast(msg, type);
        (host != null ? host.Toast : UiProperty.Toast).Show(toast, toast.Type, entry, classes: ["Light"],
            onClick: clickAction, showClose: false, touchClose: true,
            expiration: time ?? TimeSpan.FromSeconds(3.0), operateButtons: operateButtons,
            isButtonsInline: isButtonsInline, onClose: closeAction, showIcon: true);
    }

    public static void NotificationCard(string msg, NotificationType type, string title, NotificationEntry entry,
        Action closeAction,
        TimeSpan? time = null,
        Action? onClick = null, IMirelWindow? host = null, IList<OperateButtonEntry>? operateButtons = null,
        bool isButtonsInline = false)
    {
        var notification = new Notification(title, msg, type);
        // WindowNotificationManager 是第三方库，无法直接传递 NotificationEntry
        // 通知卡片模式下，关闭操作由 closeAction 处理
        (host != null ? host.Notification : UiProperty.Notification).Show(notification, notification.Type,
            showClose: false,
            classes: new[] { "Light" }, onClick: () =>
            {
                closeAction.Invoke();
                onClick?.Invoke();
            },
            expiration: time ?? TimeSpan.FromSeconds(3.0));
    }

    public static async Task OpenFolder(string path)
    {
        if (Data.DesktopType == DesktopType.MacOs)
        {
            var process = new Process();
            process.StartInfo.FileName = "open";
            process.StartInfo.Arguments = $"\"{path}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }
        else
        {
            var launcher = Mirel.App.TopLevel.Launcher;
            await launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(path));
        }
    }


    public static string GetHostId(Control sender)
    {
        var vis = sender.GetVisualRoot();
        return vis is TabWindow w
            ? w.DialogHost.HostId
            : "MainWindow";
    }
}