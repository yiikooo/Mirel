using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Mirel.Classes.Entries;
using Mirel.Controls;
using Ursa.Controls;

namespace Mirel.Module.Ui.Helper;

public class MirelWindowToastManager : WindowMessageManager, IToastManager
{
    public MirelWindowToastManager()
    {
    }

    public MirelWindowToastManager(TopLevel? host) : this()
    {
        if (host is not null)
        {
            InstallFromTopLevel(host);
        }
    }

    public MirelWindowToastManager(VisualLayerManager? visualLayerManager) : base(visualLayerManager)
    {
    }

    public static bool TryGetToastManager(Visual? visual, out WindowToastManager? manager)
    {
        manager = visual?.FindDescendantOfType<WindowToastManager>();
        return manager is not null;
    }

    public void Show(IToast content)
    {
        Show(content, content.Type, null, content.Expiration,
            content.ShowIcon, content.ShowClose,
            true, content.OnClick, content.OnClose);
    }

    public override void Show(object content)
    {
        if (content is IToast toast)
        {
            Show(toast, toast.Type, null, toast.Expiration,
                toast.ShowIcon, toast.ShowClose,
                true, toast.OnClick, toast.OnClose);
        }
        else
        {
            Show(content, NotificationType.Information);
        }
    }

    public async void Show(
        object content,
        NotificationType type,
        NotificationEntry? notificationEntry = null,
        TimeSpan? expiration = null,
        bool showIcon = true,
        bool showClose = true,
        bool touchClose = false,
        Action? onClick = null,
        Action? onClose = null,
        string[]? classes = null,
        IList<OperateButtonEntry>? operateButtons = null,
        bool isButtonsInline = false)
    {
        Dispatcher.UIThread.VerifyAccess();

        var toastControl = new MirelToastCard
        {
            Content = content,
            NotificationType = type,
            ShowIcon = showIcon,
            ShowClose = showClose,
            OperateButtons = operateButtons,
            IsButtonsInline = isButtonsInline,
            NotificationEntry = notificationEntry
        };

        if (classes is not null)
        {
            foreach (var @class in classes)
            {
                toastControl.Classes.Add(@class);
            }
        }

        toastControl.MessageClosed += (sender, _) =>
        {
            onClose?.Invoke();

            _items?.Remove(sender);
        };

        toastControl.PointerPressed += (_, _) =>
        {
            if (touchClose)
                toastControl.Close();
            onClick?.Invoke();
        };

        Dispatcher.UIThread.Post(() =>
        {
            _items?.Add(toastControl);

            if (_items?.OfType<MirelToastCard>().Count(i => !i.IsClosing) > MaxItems)
            {
                _items.OfType<MirelToastCard>().First(i => !i.IsClosing).Close();
            }
        });

        if (expiration == TimeSpan.Zero)
        {
            return;
        }

        await Task.Delay(expiration ?? TimeSpan.FromSeconds(3));

        toastControl.Close();
    }
}