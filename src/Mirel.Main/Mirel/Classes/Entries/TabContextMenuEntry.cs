using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Media;

namespace Mirel.Classes.Entries;

/// <summary>
/// 标签页右键菜单项配置
/// </summary>
public class TabContextMenuEntry
{
    /// <summary>
    /// 菜单项标题
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    /// 菜单项图标（SVG Path Data）
    /// </summary>
    public Geometry? Icon { get; set; }

    /// <summary>
    /// 快捷键提示文本
    /// </summary>
    public string? InputGesture { get; set; }

    /// <summary>
    /// 菜单项命令
    /// </summary>
    public ICommand? Command { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 子菜单项列表
    /// </summary>
    public List<TabContextMenuEntry>? SubItems { get; set; }

    /// <summary>
    /// 是否为分隔符
    /// </summary>
    public bool IsSeparator { get; set; }

    /// <summary>
    /// 菜单项分组名称（用于组织菜单结构）
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// 菜单项排序优先级（数字越小越靠前）
    /// </summary>
    public int Priority { get; set; } = 100;
}
