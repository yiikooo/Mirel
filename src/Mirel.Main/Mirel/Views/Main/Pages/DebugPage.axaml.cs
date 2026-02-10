using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module;
using Mirel.Module.Ui;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages;

public partial class DebugPage : PageModelBase, IMirelTabPage, IMirelNavPage, IMirelTabContextMenuProvider
{
    public DebugPage()
    {
        InitializeComponent();
        DataContext = this;
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        PageInfo = new PageInfoEntry
        {
            Icon = StreamGeometry.Parse(
                "M256 0c53 0 96 43 96 96l0 3.6c0 15.7-12.7 28.4-28.4 28.4l-135.1 0c-15.7 0-28.4-12.7-28.4-28.4l0-3.6c0-53 43-96 96-96zM41.4 105.4c12.5-12.5 32.8-12.5 45.3 0l64 64c.7 .7 1.3 1.4 1.9 2.1c14.2-7.3 30.4-11.4 47.5-11.4l112 0c17.1 0 33.2 4.1 47.5 11.4c.6-.7 1.2-1.4 1.9-2.1l64-64c12.5-12.5 32.8-12.5 45.3 0s12.5 32.8 0 45.3l-64 64c-.7 .7-1.4 1.3-2.1 1.9c6.2 12 10.1 25.3 11.1 39.5l64.3 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-64 0c0 24.6-5.5 47.8-15.4 68.6c2.2 1.3 4.2 2.9 6 4.8l64 64c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0l-63.1-63.1c-24.5 21.8-55.8 36.2-90.3 39.6L272 240c0-8.8-7.2-16-16-16s-16 7.2-16 16l0 239.2c-34.5-3.4-65.8-17.8-90.3-39.6L86.6 502.6c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3l64-64c1.9-1.9 3.9-3.4 6-4.8C101.5 367.8 96 344.6 96 320l-64 0c-17.7 0-32-14.3-32-32s14.3-32 32-32l64.3 0c1.1-14.1 5-27.5 11.1-39.5c-.7-.6-1.4-1.2-2.1-1.9l-64-64c-12.5-12.5-12.5-32.8 0-45.3z"),
            Title = "调试"
        };
    }

    public string ShortInfo { get; set; }

    public static MirelStaticPageInfo StaticPageInfo { get; } = new()
    {
        Title = "调试",
        Icon = StreamGeometry.Parse(
            "M256 0c53 0 96 43 96 96l0 3.6c0 15.7-12.7 28.4-28.4 28.4l-135.1 0c-15.7 0-28.4-12.7-28.4-28.4l0-3.6c0-53 43-96 96-96zM41.4 105.4c12.5-12.5 32.8-12.5 45.3 0l64 64c.7 .7 1.3 1.4 1.9 2.1c14.2-7.3 30.4-11.4 47.5-11.4l112 0c17.1 0 33.2 4.1 47.5 11.4c.6-.7 1.2-1.4 1.9-2.1l64-64c12.5-12.5 32.8-12.5 45.3 0s12.5 32.8 0 45.3l-64 64c-.7 .7-1.4 1.3-2.1 1.9c6.2 12 10.1 25.3 11.1 39.5l64.3 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-64 0c0 24.6-5.5 47.8-15.4 68.6c2.2 1.3 4.2 2.9 6 4.8l64 64c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0l-63.1-63.1c-24.5 21.8-55.8 36.2-90.3 39.6L272 240c0-8.8-7.2-16-16-16s-16 7.2-16 16l0 239.2c-34.5-3.4-65.8-17.8-90.3-39.6L86.6 502.6c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3l64-64c1.9-1.9 3.9-3.4 6-4.8C101.5 367.8 96 344.6 96 320l-64 0c-17.7 0-32-14.3-32-32s14.3-32 32-32l64.3 0c1.1-14.1 5-27.5 11.1-39.5c-.7-.6-1.4-1.2-2.1-1.9l-64-64c-12.5-12.5-12.5-32.8 0-45.3z")
    };

    public static IMirelPage Create(object sender, object? param = null)
    {
        return new DebugPage();
    }

    public List<TabContextMenuEntry>? GetCustomContextMenuItems()
    {
        return
        [
            new TabContextMenuEntry
            {
                Header = "🐛 调试专用功能",
                Icon = Geometry.Parse(
                    "M256 0c53 0 96 43 96 96l0 3.6c0 15.7-12.7 28.4-28.4 28.4l-135.1 0c-15.7 0-28.4-12.7-28.4-28.4l0-3.6c0-53 43-96 96-96z"),
                Command = new RelayCommand(() => { Overlay.Notice("这是调试页面的自定义菜单项！", NotificationType.Information); }),
                Priority = 10
            },

            new TabContextMenuEntry
            {
                Header = "📊 查看调试信息",
                Icon = Geometry.Parse(
                    "M160 80c0-26.5 21.5-48 48-48l32 0c26.5 0 48 21.5 48 48l0 352c0 26.5-21.5 48-48 48l-32 0c-26.5 0-48-21.5-48-48l0-352zM0 272c0-26.5 21.5-48 48-48l32 0c26.5 0 48 21.5 48 48l0 160c0 26.5-21.5 48-48 48l-32 0c-26.5 0-48-21.5-48-48L0 272zM368 96l32 0c26.5 0 48 21.5 48 48l0 288c0 26.5-21.5 48-48 48l-32 0c-26.5 0-48-21.5-48-48l0-288c0-26.5 21.5-48 48-48z"),
                Command = new RelayCommand(() =>
                {
                    Logger.Debug("查看调试信息被点击");
                    Overlay.Notice("调试信息已输出到日志", NotificationType.Success);
                }),
                Priority = 20
            },

            new TabContextMenuEntry
            {
                IsSeparator = true,
                Priority = 30
            },

            new TabContextMenuEntry
            {
                Header = "🔧 高级选项",
                Icon = Geometry.Parse(
                    "M495.9 166.6c3.2 8.7 .5 18.4-6.4 24.6l-43.3 39.4c1.1 8.3 1.7 16.8 1.7 25.4s-.6 17.1-1.7 25.4l43.3 39.4c6.9 6.2 9.6 15.9 6.4 24.6c-4.4 11.9-9.7 23.3-15.8 34.3l-4.7 8.1c-6.6 11-14 21.4-22.1 31.2c-5.9 7.2-15.7 9.6-24.5 6.8l-55.7-17.7c-13.4 10.3-28.2 18.9-44 25.4l-12.5 57.1c-2 9.1-9 16.3-18.2 17.8c-13.8 2.3-28 3.5-42.5 3.5s-28.7-1.2-42.5-3.5c-9.2-1.5-16.2-8.7-18.2-17.8l-12.5-57.1c-15.8-6.5-30.6-15.1-44-25.4L83.1 425.9c-8.8 2.8-18.6 .3-24.5-6.8c-8.1-9.8-15.5-20.2-22.1-31.2l-4.7-8.1c-6.1-11-11.4-22.4-15.8-34.3c-3.2-8.7-.5-18.4 6.4-24.6l43.3-39.4C64.6 273.1 64 264.6 64 256s.6-17.1 1.7-25.4L22.4 191.2c-6.9-6.2-9.6-15.9-6.4-24.6c4.4-11.9 9.7-23.3 15.8-34.3l4.7-8.1c6.6-11 14-21.4 22.1-31.2c5.9-7.2 15.7-9.6 24.5-6.8l55.7 17.7c13.4-10.3 28.2-18.9 44-25.4l12.5-57.1c2-9.1 9-16.3 18.2-17.8C227.3 1.2 241.5 0 256 0s28.7 1.2 42.5 3.5c9.2 1.5 16.2 8.7 18.2 17.8l12.5 57.1c15.8 6.5 30.6 15.1 44 25.4l55.7-17.7c8.8-2.8 18.6-.3 24.5 6.8c8.1 9.8 15.5 20.2 22.1 31.2l4.7 8.1c6.1 11 11.4 22.4 15.8 34.3zM256 336a80 80 0 1 0 0-160 80 80 0 1 0 0 160z"),
                SubItems =
                [
                    new TabContextMenuEntry
                    {
                        Header = "清空日志",
                        Command = new RelayCommand(() =>
                        {
                            Logger.Debug("清空日志功能");
                            Overlay.Notice("日志已清空", NotificationType.Warning);
                        })
                    },

                    new TabContextMenuEntry
                    {
                        Header = "重置设置",
                        Command = new RelayCommand(() =>
                        {
                            Logger.Debug("重置设置功能");
                            Overlay.Notice("设置已重置", NotificationType.Warning);
                        })
                    },

                    new TabContextMenuEntry
                    {
                        IsSeparator = true
                    },

                    new TabContextMenuEntry
                    {
                        Header = "导出调试数据",
                        Command = new RelayCommand(() =>
                        {
                            Logger.Debug("导出调试数据");
                            Overlay.Notice("调试数据已导出", NotificationType.Success);
                        })
                    }
                ],
                Priority = 40
            }
        ];
    }

    public List<TabContextMenuGroup>? GetCustomContextMenuGroups()
    {
        // 使用分组方式组织菜单（这里返回null，使用上面的简单列表方式）
        // 如果想使用分组方式，可以返回如下结构：
        /*
        return new List<TabContextMenuGroup>
        {
            new TabContextMenuGroup
            {
                Name = "调试工具",
                Priority = 10,
                Items = new List<TabContextMenuEntry>
                {
                    new TabContextMenuEntry { Header = "功能1", ... },
                    new TabContextMenuEntry { Header = "功能2", ... }
                },
                AddSeparatorAfter = true
            },
            new TabContextMenuGroup
            {
                Name = "高级功能",
                Priority = 20,
                Items = new List<TabContextMenuEntry>
                {
                    new TabContextMenuEntry { Header = "高级功能1", ... }
                }
            }
        };
        */
        return null;
    }

    public bool ReplaceDefaultMenu => false; // false表示追加到默认菜单后面，true表示完全替换默认菜单

    public Control RootElement { get; init; }
    public PageLoadingAnimator InAnimator { get; set; }

    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }

    public void OnClose()
    {
    }

    private void Notice(object? sender, RoutedEventArgs e)
    {
        var t = ((Button)sender).Tag.ToString();
        var i = new TimeSpan(1, 0, 3);
        switch (t)
        {
            case "Info":
                Overlay.Notice("Info");
                break;
            case "Success":
                Overlay.Notice("Success", NotificationType.Success, new NoticeOptions
                {
                    Expiration = i
                });
                break;
            case "Warn":
                Overlay.Notice("Warn", NotificationType.Warning);
                break;
            case "Error":
                Overlay.Notice("Error", NotificationType.Error);
                break;
            case "Long":
                Overlay.Notice(
                    "Avalonia 是一个基于 .NET 的跨平台 UI 框架，灵感来源于 WPF，可在 Windows、macOS、Linux、移动设备和 WebAssembly 上使用同一套 XAML 代码开发应用程序，适合桌面和移动端开发者探索跨平台解决方案。",
                    NotificationType.Information,
                    new NoticeOptions
                    {
                        Expiration = i
                    });
                break;
            case "Click":
                Overlay.Notice("Avalonia", NotificationType.Information, new NoticeOptions
                {
                    OnClick = () => { Logger.Debug("OnClick!"); }
                });
                break;
        }
    }

    private void NoticeWithButtons(object? sender, RoutedEventArgs e)
    {
        var buttons = new ObservableCollection<OperateButtonEntry>
        {
            new("查看详情", _ => { Logger.Debug("查看详情按钮被点击"); }, false),
            new("关闭", _ => { Logger.Debug("关闭按钮被点击"); }, true),
            new("关闭并移除", _ => { Logger.Debug("关闭并移除按钮被点击"); }, true,
                true)
        };

        Overlay.Notice(
            "这是一条带有操作按钮的通知",
            NotificationType.Information,
            new NoticeOptions
            {
                OperateButtons = buttons
            }
        );
    }


    private void NoticeWithButtonsInline(object? sender, RoutedEventArgs e)
    {
        var buttons = new ObservableCollection<OperateButtonEntry>
        {
            new("查看详情", _ => { Logger.Debug("查看详情按钮被点击"); }, false),
            new("关闭", _ => { Logger.Debug("关闭按钮被点击"); }, true),
            new("关闭并移除", _ => { Logger.Debug("关闭并移除按钮被点击"); }, true,
                true)
        };

        Overlay.Notice(
            "这是一条按钮在同一行的通知",
            NotificationType.Information,
            new NoticeOptions
            {
                OperateButtons = buttons,
                IsButtonsInline = true,
                Expiration = new TimeSpan(0, 2, 0, 0, 0)
            }
        );
    }


    private void Crush(object? sender, RoutedEventArgs e)
    {
        var a = 0;
        // ReSharper disable once IntDivisionByZero
        var b = 1 / a;
    }
}