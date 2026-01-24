using Avalonia.Controls;
using Mirel.Module.Events;

namespace Mirel.Views.Main;

public partial class MoreButtonMenu : UserControl
{
    public MenuFlyout MenuFlyout => (MenuFlyout)MainControl.Flyout;
    public MoreButtonMenu()
    {
        InitializeComponent();
        InitEvents.OnMoreMenuLoaded(this);
    }
}