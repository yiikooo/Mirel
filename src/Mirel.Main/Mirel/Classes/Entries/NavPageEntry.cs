using Mirel.Classes.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mirel.Classes.Entries;

public class NavPageEntry
{
    [JsonProperty]
    [JsonConverter(typeof(StringEnumConverter))]
    public PageIdentifier Identifier { get; set; }

    [JsonProperty] public string DisplayName { get; set; }

    [JsonProperty] public bool IsEnabled { get; set; } = true;

    [JsonProperty] public int Order { get; set; }
}