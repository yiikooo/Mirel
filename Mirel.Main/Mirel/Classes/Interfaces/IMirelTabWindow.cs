using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

public interface IMirelTabWindow : IMirelWindow
{
    public ObservableCollection<TabEntry> Tabs { get; }
}