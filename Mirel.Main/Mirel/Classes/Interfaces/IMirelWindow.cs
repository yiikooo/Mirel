using Avalonia.Controls;
using Mirel.Module.Ui.Helper;
using Ursa.Controls;

namespace Mirel.Classes.Interfaces;

public interface IMirelWindow 
{
    public WindowNotificationManager Notification { get; set; }
    public MirelWindowToastManager Toast { get; set; }
    public Control RootElement { get; set; }
    public UrsaWindow Window { get; set; }
}