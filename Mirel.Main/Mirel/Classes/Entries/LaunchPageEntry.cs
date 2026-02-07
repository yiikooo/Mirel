using Mirel.Classes.Interfaces;
using Newtonsoft.Json;

namespace Mirel.Classes.Entries;

public class LaunchPageEntry
{
    public string Id { get; init; }

    [JsonIgnore] public string Header { get; set; }

    public string Tag { get; set; } // 每个窗口中只能存在一个相同标签

    [JsonIgnore] public IMirelTabPage Page { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not LaunchPageEntry entry) return false;
        return entry.Id == Id;
    }
}