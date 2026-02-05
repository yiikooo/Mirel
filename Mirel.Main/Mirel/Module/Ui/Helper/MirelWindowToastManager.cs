using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
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
        Show(content, content.Type, content.Expiration,
            content.ShowIcon, content.ShowClose,
            true, content.OnClick, content.OnClose);
    }

    public override void Show(object content)
    {
        if (content is IToast toast)
        {
            Show(toast, toast.Type, toast.Expiration,
                toast.ShowIcon, toast.ShowClose,
                true, toast.OnClick, toast.OnClose);
        }
        else
        {
            Show(content, NotificationType.Information);
        }
    }
    
    public new async void Show(
        object content,
        NotificationType type,
        TimeSpan? expiration = null,
        bool showIcon = true,
        bool showClose = true,
        bool touchClose = false,
        Action? onClick = null,
        Action? onClose = null,
        string[]? classes = null)
    {
        Dispatcher.UIThread.VerifyAccess();

        var toastControl = new MirelToastCard
        {
            Content = content,
            NotificationType = type,
            ShowIcon = showIcon,
            ShowClose = showClose
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
            if(touchClose)
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