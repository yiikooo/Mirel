using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;

namespace Mirel.Module.Ui;

/// <summary>
/// 通知选项
/// </summary>
public class NoticeOptions(IMirelWindow? host = null, Action? onClose = null)
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public NotificationType Type { get; set; } = NotificationType.Information;

    /// <summary>
    /// 显示时长
    /// </summary>
    public TimeSpan? Expiration { get; init; }

    /// <summary>
    /// 点击回调
    /// </summary>
    public Action? OnClick { get; init; }

    /// <summary>
    /// 关闭回调
    /// </summary>
    public Action? OnClose { get; set; } = onClose;

    /// <summary>
    /// 宿主窗口
    /// </summary>
    public IMirelWindow? Host { get; set; } = host;

    /// <summary>
    /// 操作按钮
    /// </summary>
    public IList<OperateButtonEntry>? OperateButtons { get; init; }

    /// <summary>
    /// 按钮是否内联显示
    /// </summary>
    public bool IsButtonsInline { get; init; }
}
