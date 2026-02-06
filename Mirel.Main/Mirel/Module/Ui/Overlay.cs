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
using Mirel.Module.Ui.Helper;
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

    public static void Notice(string msg, NotificationType type = NotificationType.Information, NoticeOptions? options = null)
    {
        options ??= new NoticeOptions();
        options.Type = type;

        var t = DateTime.Now;
        Logger.Info($"[Notice] [{type}] {msg}");

        var notification = new Notification("Mirel", msg, type);
        var entry = new NotificationEntry(notification, notification.Type, t, operateButtons: options.OperateButtons);

        UiProperty.Notifications.Insert(0, entry);
        UiProperty.HistoryNotifications.Insert(0, entry);

        ShowToast(msg, entry, options);
    }

    public static void ShowToast(string msg, NotificationEntry entry, NoticeOptions? options = null)
    {
        options ??= new NoticeOptions();

        var toast = new Toast(msg, options.Type);
        var toastOptions = new ToastOptions
        {
            Type = options.Type,
            NotificationEntry = entry,
            Classes = ["Light"],
            OnClick = options.OnClick,
            ShowClose = false,
            TouchClose = true,
            Expiration = options.Expiration ?? TimeSpan.FromSeconds(3.0),
            OperateButtons = options.OperateButtons,
            IsButtonsInline = options.IsButtonsInline,
            OnClose = options.OnClose,
            ShowIcon = true
        };

        var toastManager = options.Host?.Toast ?? UiProperty.Toast;
        toastManager.Show(toast, toastOptions);
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