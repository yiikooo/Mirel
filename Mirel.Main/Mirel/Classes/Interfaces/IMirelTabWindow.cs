using System.Collections.ObjectModel;
using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelTabWindow : IMirelWindow
{
    public ObservableCollection<TabEntry> Tabs { get; }
    public TabEntry SelectedTab { get; }
    public string WindowId { get; init; }
    public void SelectTab(TabEntry tab);
}