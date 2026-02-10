using System;
using System.Linq;
using Mirel.Classes.Enums;

namespace Mirel.Classes.Entries;

/// <summary>
/// 聚合搜索类型信息，包含搜索关键词
/// </summary>
public record AggregateSearchTypeInfo
{
    public static readonly AggregateSearchTypeInfo[] AllTypes =
    [
        new()
        {
            Type = AggregateSearchType.Tab,
            DisplayName = "标签页",
            Keywords = ["标签页 ", "tab", "bqy"]
        },
        new()
        {
            Type = AggregateSearchType.Page,
            DisplayName = "页面",
            Keywords = ["页面", "page", "ym"]
        }
    ];

    public AggregateSearchType Type { get; init; }
    public string DisplayName { get; init; }
    public string[] Keywords { get; init; }

    /// <summary>
    /// 根据关键词查找搜索类型
    /// </summary>
    public static AggregateSearchType? FindTypeByKeyword(string keyword)
    {
        var lowerKeyword = keyword.ToLowerInvariant();
        foreach (var typeInfo in AllTypes)
        {
            if (typeInfo.Keywords.Any(k => (k).Equals(lowerKeyword, StringComparison.OrdinalIgnoreCase)))
            {
                return typeInfo.Type;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据类型获取显示名称
    /// </summary>
    public static string GetDisplayName(AggregateSearchType type)
    {
        return AllTypes.FirstOrDefault(t => t.Type == type)?.DisplayName ?? "全部";
    }
}