using Mirel.Classes.Entries;
using Mirel.Module.Ui.Helper;

namespace Mirel.Classes.Interfaces;

public interface IMirelTabPage : IMirelPage
{
    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }
    public PageLoadingAnimator InAnimator { get; set; }

    public void OnClose();
}