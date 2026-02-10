using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.Service;
using Mirel.Views.Main;

namespace Mirel.Module.Ui.Helper;

/// <summary>
/// 标签页右键菜单构建器
/// </summary>
public static class TabContextMenuBuilder
{
    /// <summary>
    /// 构建标签页右键菜单
    /// </summary>
    /// <param name="menuFlyout">要填充的菜单</param>
    /// <param name="tabEntry">标签页实体</param>
    /// <param name="sourceWindow">来源窗口</param>
    public static void BuildContextMenu(MenuFlyout menuFlyout, TabEntry tabEntry, IMirelTabWindow? sourceWindow = null)
    {
        menuFlyout.Items.Clear();

        // 检查页面是否实现了自定义菜单接口
        if (tabEntry.Content is IMirelTabContextMenuProvider menuProvider)
        {
            // 如果完全替换默认菜单
            if (menuProvider.ReplaceDefaultMenu)
            {
                BuildCustomMenu(menuFlyout, menuProvider, tabEntry, sourceWindow);
                return;
            }

            // 否则先添加默认菜单，再追加自定义菜单
            BuildDefaultMenu(menuFlyout, tabEntry, sourceWindow);
            AppendCustomMenu(menuFlyout, menuProvider, tabEntry, sourceWindow);
        }
        else
        {
            // 使用默认菜单
            BuildDefaultMenu(menuFlyout, tabEntry, sourceWindow);
        }
    }

    /// <summary>
    /// 构建默认菜单
    /// </summary>
    private static void BuildDefaultMenu(MenuFlyout menuFlyout, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        // 关闭标签页
        var closeMenuItem = new MenuItem
        {
            Header = "关闭标签页",
            InputGesture = KeyGesture.Parse("Ctrl + W"),
            Icon = new PathIcon
            {
                Data = Geometry.Parse("M7.46,11.88L8.88,10.46L11,12.59L13.12,10.46L14.54,11.88L12.41,14L14.54,16.12L13.12,17.54L11,15.41L8.88,17.54L7.46,16.12L9.59,14L7.46,11.88M3,3H21A2,2 0 0,1 23,5V19A2,2 0 0,1 21,21H3A2,2 0 0,1 1,19V5A2,2 0 0,1 3,3M3,5V19H21V9H13V5H3Z"),
                Width = 17
            },
            Command = new RelayCommand(() =>
            {
                if (tabEntry.CanClose)
                {
                    if (sourceWindow is TabWindow tabWindow)
                    {
                        tabWindow.RemoveTab(tabEntry);
                    }
                    else if (sourceWindow != null)
                    {
                        sourceWindow.Tabs.Remove(tabEntry);
                    }

                    tabEntry.DisposeContent();
                    tabEntry.Removing();
                }
            }),
            IsEnabled = tabEntry.CanClose
        };
        menuFlyout.Items.Add(closeMenuItem);

        // 在新窗口中打开
        var openInNewWindowMenuItem = new MenuItem
        {
            Header = "在新窗口中打开",
            Icon = new PathIcon
            {
                Data = Geometry.Parse("M12,10L8,14H11V20H13V14H16M19,4H5C3.89,4 3,4.9 3,6V18A2,2 0 0,0 5,20H9V18H5V8H19V18H15V20H19A2,2 0 0,0 21,18V6A2,2 0 0,0 19,4Z"),
                Width = 17
            },
            Command = new RelayCommand(() => { tabEntry.MoveTabToNewWindow(); })
        };
        menuFlyout.Items.Add(openInNewWindowMenuItem);

        // 移动到窗口
        var moveToWindowMenuItem = new MenuItem
        {
            Header = "移动到窗口",
            Icon = new PathIcon
            {
                Data = Geometry.Parse("M14,14H16L12,10L8,14H10V18H14M20,4H4A2,2 0 0,0 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V6A2,2 0 0,0 20,4M20,18H4V6H20V18Z"),
                Width = 17
            }
        };

        // 获取所有窗口
        var allWindows = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.Windows.OfType<IMirelTabWindow>().ToList() ?? new List<IMirelTabWindow>();

        var otherWindows = allWindows.Where(w => w != sourceWindow).ToList();

        if (otherWindows.Count == 0)
        {
            var noWindowItem = new MenuItem
            {
                Header = "没有其他窗口",
                IsEnabled = false,
                Icon = new PathIcon
                {
                    Data = Geometry.Parse("F1 M512,512z M0,0z M502.6,278.6C515.1,266.1,515.1,245.8,502.6,233.3L342.6,73.3C330.1,60.8 309.8,60.8 297.3,73.3 284.8,85.8 284.8,106.1 297.3,118.6L402.7,224 32,224C14.3,224 0,238.3 0,256 0,273.7 14.3,288 32,288L402.7,288 297.3,393.4C284.8,405.9 284.8,426.2 297.3,438.7 309.8,451.2 330.1,451.2 342.6,438.7L502.6,278.7z"),
                    Width = 14,
                    Margin = new Thickness(8, -1, 0, 0)
                }
            };
            moveToWindowMenuItem.Items.Add(noWindowItem);
        }
        else
        {
            foreach (var window in otherWindows)
            {
                var menuItem = new MenuItem
                {
                    Header = window.WindowId,
                    Icon = new PathIcon
                    {
                        Data = Geometry.Parse("F1 M512,512z M0,0z M502.6,278.6C515.1,266.1,515.1,245.8,502.6,233.3L342.6,73.3C330.1,60.8 309.8,60.8 297.3,73.3 284.8,85.8 284.8,106.1 297.3,118.6L402.7,224 32,224C14.3,224 0,238.3 0,256 0,273.7 14.3,288 32,288L402.7,288 297.3,393.4C284.8,405.9 284.8,426.2 297.3,438.7 309.8,451.2 330.1,451.2 342.6,438.7L502.6,278.7z"),
                        Width = 14,
                        Margin = new Thickness(8, -1, 0, 0)
                    },
                    Command = new RelayCommand(() =>
                    {
                        if (window is Window targetWindow)
                        {
                            tabEntry.MoveTabToWindow(targetWindow);
                        }
                    })
                };
                moveToWindowMenuItem.Items.Add(menuItem);
            }
        }

        menuFlyout.Items.Add(moveToWindowMenuItem);
    }

    /// <summary>
    /// 构建完全自定义的菜单
    /// </summary>
    private static void BuildCustomMenu(MenuFlyout menuFlyout, IMirelTabContextMenuProvider menuProvider, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        // 优先使用分组方式
        var groups = menuProvider.GetCustomContextMenuGroups();
        if (groups != null && groups.Count > 0)
        {
            BuildMenuFromGroups(menuFlyout, groups, tabEntry, sourceWindow);
            return;
        }

        // 否则使用简单列表方式
        var items = menuProvider.GetCustomContextMenuItems();
        if (items != null && items.Count > 0)
        {
            BuildMenuFromItems(menuFlyout, items, tabEntry, sourceWindow);
        }
    }

    /// <summary>
    /// 追加自定义菜单到默认菜单后面
    /// </summary>
    private static void AppendCustomMenu(MenuFlyout menuFlyout, IMirelTabContextMenuProvider menuProvider, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        // 优先使用分组方式
        var groups = menuProvider.GetCustomContextMenuGroups();
        if (groups != null && groups.Count > 0)
        {
            // 添加分隔符
            menuFlyout.Items.Add(new Separator());
            BuildMenuFromGroups(menuFlyout, groups, tabEntry, sourceWindow);
            return;
        }

        // 否则使用简单列表方式
        var items = menuProvider.GetCustomContextMenuItems();
        if (items != null && items.Count > 0)
        {
            // 添加分隔符
            menuFlyout.Items.Add(new Separator());
            BuildMenuFromItems(menuFlyout, items, tabEntry, sourceWindow);
        }
    }

    /// <summary>
    /// 从分组构建菜单
    /// </summary>
    private static void BuildMenuFromGroups(MenuFlyout menuFlyout, List<TabContextMenuGroup> groups, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        // 按优先级排序
        var sortedGroups = groups.OrderBy(g => g.Priority).ToList();

        for (var i = 0; i < sortedGroups.Count; i++)
        {
            var group = sortedGroups[i];

            // 添加该分组的所有菜单项
            BuildMenuFromItems(menuFlyout, group.Items, tabEntry, sourceWindow);

            // 如果需要在分组后添加分隔符，且不是最后一个分组
            if (group.AddSeparatorAfter && i < sortedGroups.Count - 1)
            {
                menuFlyout.Items.Add(new Separator());
            }
        }
    }

    /// <summary>
    /// 从菜单项列表构建菜单
    /// </summary>
    private static void BuildMenuFromItems(MenuFlyout menuFlyout, List<TabContextMenuEntry> items, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        // 按优先级排序
        var sortedItems = items.OrderBy(item => item.Priority).ToList();

        foreach (var item in sortedItems)
        {
            if (!item.IsVisible) continue;

            // 如果是分隔符
            if (item.IsSeparator)
            {
                menuFlyout.Items.Add(new Separator());
                continue;
            }

            var menuItem = CreateMenuItem(item, tabEntry, sourceWindow);
            menuFlyout.Items.Add(menuItem);
        }
    }

    /// <summary>
    /// 创建菜单项
    /// </summary>
    private static MenuItem CreateMenuItem(TabContextMenuEntry entry, TabEntry tabEntry, IMirelTabWindow? sourceWindow)
    {
        var menuItem = new MenuItem
        {
            Header = entry.Header,
            Command = entry.Command,
            IsEnabled = entry.IsEnabled,
            IsVisible = entry.IsVisible
        };

        // 设置图标
        if (entry.Icon != null)
        {
            menuItem.Icon = new PathIcon
            {
                Data = entry.Icon,
                Width = 17
            };
        }

        // 设置快捷键提示
        if (!string.IsNullOrEmpty(entry.InputGesture))
        {
            menuItem.InputGesture = KeyGesture.Parse(entry.InputGesture);
        }

        // 如果有子菜单项
        if (entry.SubItems != null && entry.SubItems.Count > 0)
        {
            foreach (var subEntry in entry.SubItems.OrderBy(s => s.Priority))
            {
                if (!subEntry.IsVisible) continue;

                if (subEntry.IsSeparator)
                {
                    menuItem.Items.Add(new Separator());
                }
                else
                {
                    var subMenuItem = CreateMenuItem(subEntry, tabEntry, sourceWindow);
                    menuItem.Items.Add(subMenuItem);
                }
            }
        }

        return menuItem;
    }
}
