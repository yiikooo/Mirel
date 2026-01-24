using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelTabPage : IMirelPage
{
    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }
    public void OnClose();
}