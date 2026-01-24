using Avalonia.Controls;
using Mirel.Module.Ui.Helper;

namespace Mirel.Classes.Interfaces;

public interface IMirelPage
{
    public string ShortInfo { get; set; } 
    public Control BottomElement { get; set; } 
    public Control RootElement { get; set; }
    public PageLoadingAnimator InAnimator { get; set; }
}