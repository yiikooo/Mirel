using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Mirel.Controls;
using Mirel.Module.Service;
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
        var options = new ToastOptions
        {
            Type = content.Type,
            Expiration = content.Expiration,
            ShowIcon = content.ShowIcon,
            ShowClose = content.ShowClose,
            TouchClose = true,
            OnClick = content.OnClick,
            OnClose = content.OnClose
        };
        Show(content, options);
    }

    public override void Show(object content)
    {
        if (content is IToast toast)
        {
            Show(toast);
        }
        else
        {
            Show(content, new ToastOptions());
        }
    }

    /// <summary>
    /// 显示 Toast 通知
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="options">显示选项</param>
    public async void Show(object content, ToastOptions options)
    {
        try
        {
            Dispatcher.UIThread.VerifyAccess();

            var toastControl = new MirelToastCard
            {
                Content = content,
                NotificationType = options.Type,
                ShowIcon = options.ShowIcon,
                ShowClose = options.ShowClose,
                OperateButtons = options.OperateButtons,
                IsButtonsInline = options.IsButtonsInline,
                NotificationEntry = options.NotificationEntry
            };

            if (options.Classes is not null)
            {
                foreach (var @class in options.Classes)
                {
                    toastControl.Classes.Add(@class);
                }
            }

            toastControl.MessageClosed += (sender, _) =>
            {
                options.OnClose?.Invoke();
                _items?.Remove(sender);
            };

            toastControl.PointerPressed += (_, _) =>
            {
                if (options.TouchClose)
                    toastControl.Close();
                options.OnClick?.Invoke();
            };

            Dispatcher.UIThread.Post(() =>
            {
                _items?.Add(toastControl);

                if (_items?.OfType<MirelToastCard>().Count(i => !i.IsClosing) > MaxItems)
                {
                    _items.OfType<MirelToastCard>().First(i => !i.IsClosing).Close();
                }
            });

            if (options.Expiration == TimeSpan.Zero)
            {
                return;
            }

            await Task.Delay(options.Expiration ?? TimeSpan.FromSeconds(3));

            toastControl.CloseWithoutRemovingFromList();
        }
        catch (Exception e)
        {
            ExceptionService.HandleException(e);
        }
    }
}