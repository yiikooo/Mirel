using System.Collections.Generic;
using Mirel.Classes.Entries;

namespace Mirel.Classes.Interfaces;

/// <summary>
/// 标签页右键菜单提供者接口
/// 实现此接口的页面可以自定义右键菜单内容
/// </summary>
public interface IMirelTabContextMenuProvider
{
    /// <summary>
    /// 获取自定义的右键菜单项列表
    /// 返回 null 表示使用默认菜单
    /// </summary>
    /// <returns>自定义菜单项列表，或 null 使用默认菜单</returns>
    List<TabContextMenuEntry>? GetCustomContextMenuItems();

    /// <summary>
    /// 获取自定义的右键菜单分组
    /// 返回 null 表示使用默认菜单
    /// </summary>
    /// <returns>自定义菜单分组列表，或 null 使用默认菜单</returns>
    List<TabContextMenuGroup>? GetCustomContextMenuGroups();

    /// <summary>
    /// 是否完全替换默认菜单
    /// true: 完全使用自定义菜单，忽略默认菜单
    /// false: 将自定义菜单项追加到默认菜单后面
    /// </summary>
    bool ReplaceDefaultMenu { get; }
}
