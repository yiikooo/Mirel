using Avalonia.Controls;
using Ursa.Controls;

namespace Mirel.Classes.Interfaces;

public interface IMirelWindow 
{
    public WindowNotificationManager Notification { get; set; }
    public WindowToastManager Toast { get; set; }
    public Control RootElement { get; set; }
    public UrsaWindow Window { get; set; }
}