using System.Collections.Generic;

namespace Mirel.Classes.Entries;

/// <summary>
/// 标签页右键菜单分组
/// </summary>
public class TabContextMenuGroup
{
    /// <summary>
    /// 分组名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 分组排序优先级（数字越小越靠前）
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// 该分组下的菜单项
    /// </summary>
    public List<TabContextMenuEntry> Items { get; set; } = new();

    /// <summary>
    /// 是否在分组后添加分隔符
    /// </summary>
    public bool AddSeparatorAfter { get; set; } = true;
}
