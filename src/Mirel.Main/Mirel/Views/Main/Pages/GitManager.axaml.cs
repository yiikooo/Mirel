using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.VisualTree;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Const;
using Mirel.Module.Ui;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;

namespace Mirel.Views.Main.Pages;

public partial class GitManager : PageModelBase, IMirelNavPage
{
    private bool _isProxyEnabled;
    private string _proxyAddress = "";
    private string _userEmail = "";
    private string _userName = "";

    public GitManager()
    {
        InitializeComponent();
        DataContext = this;
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        Loaded += async (s, e) => await LoadGitSettings();

        Root.AttachedToVisualTree += async (s, e) =>
        {
            if (Root.GetVisualRoot() is Window window)
            {
                window.Activated += async (_, _) => await LoadGitSettings();
            }
        };
    }

    public bool IsProxyEnabled
    {
        get => _isProxyEnabled;
        set
        {
            if (SetField(ref _isProxyEnabled, value))
            {
                _ = ApplyProxySettings();
            }
        }
    }

    public string ProxyAddress
    {
        get => _proxyAddress;
        set => SetField(ref _proxyAddress, value);
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (SetField(ref _userName, value))
            {
                _ = ApplyUserSettings();
            }
        }
    }

    public string UserEmail
    {
        get => _userEmail;
        set
        {
            if (SetField(ref _userEmail, value))
            {
                _ = ApplyUserSettings();
            }
        }
    }

    public Control RootElement { get; init; }
    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo => StaticPageInfo;
    public PageLoadingAnimator InAnimator { get; set; }

    public void OnClose()
    {
    }

    public static PageInfoEntry StaticPageInfo { get; } = new()
    {
        Title = "Git 管理",
        Icon = StreamGeometry.Parse(
            "F1 M448,512z M0,0z M439.6,236.1L244,40.5C238.6,35 231.2,32 223.6,32 216,32 208.6,35 203.2,40.4L162.5,81 214,132.5C241.1,123.4,266.7,149.3,257.4,176.2L307.1,225.9C341.3,214.1 368.3,256.9 342.6,282.6 316.1,309.1 272.4,279.7 286.6,245.3L240.3,199 240.3,320.9C265.6,333.4 262.6,362.7 249.4,375.9 243,382.3 234.2,386 225.1,386 216,386 207.3,382.4 200.8,375.9 183.2,358.3 189.7,329 212,319.9L212,196.9C191.2,188.4,187.4,166.2,193.4,151.9L142.6,101 8.5,235.1C3,240.6 0,247.9 0,255.5 0,263.1 3,270.5 8.5,275.9L204.1,471.6C209.5,477 216.8,480 224.5,480 232.2,480 239.5,477 244.9,471.6L439.6,276.9C445,271.5 448,264.1 448,256.5 448,248.9 445,241.5 439.6,236.1z")
    };

    public static IMirelPage Create(object sender, object? param = null)
    {
        return new GitManager();
    }

    private async Task LoadGitSettings()
    {
        await LoadProxySettings();
        await LoadUserSettings();
    }

    private async Task LoadUserSettings()
    {
        try
        {
            var userName = await ExecuteGitCommand("config --global --get user.name");
            var userEmail = await ExecuteGitCommand("config --global --get user.email");

            SetField(ref _userName, userName.Trim(), nameof(UserName));
            SetField(ref _userEmail, userEmail.Trim(), nameof(UserEmail));
        }
        catch (Exception ex)
        {
            Overlay.Notice($"加载 Git 用户设置失败: {ex.Message}", NotificationType.Error);
        }
    }

    private async Task LoadProxySettings()
    {
        try
        {
            var httpProxy = await ExecuteGitCommand("config --global --get http.proxy");
            var httpsProxy = await ExecuteGitCommand("config --global --get https.proxy");

            var currentProxy = !string.IsNullOrWhiteSpace(httpProxy) ? httpProxy : httpsProxy;

            if (!string.IsNullOrWhiteSpace(currentProxy))
            {
                SetField(ref _isProxyEnabled, true, nameof(IsProxyEnabled));
                SetField(ref _proxyAddress, currentProxy.Trim(), nameof(ProxyAddress));
            }
            else
            {
                SetField(ref _isProxyEnabled, false, nameof(IsProxyEnabled));
                SetField(ref _proxyAddress, Data.SettingEntry.GitProxyAddress, nameof(ProxyAddress));
            }
        }
        catch (Exception ex)
        {
            Overlay.Notice($"加载 Git 代理设置失败: {ex.Message}", NotificationType.Error);
        }
    }

    private async Task ApplyUserSettings()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(_userName))
            {
                await ExecuteGitCommand($"config --global user.name \"{_userName}\"");
                Overlay.Notice($"Git 用户名已设置: {_userName}", NotificationType.Success);
            }

            if (!string.IsNullOrWhiteSpace(_userEmail))
            {
                await ExecuteGitCommand($"config --global user.email \"{_userEmail}\"");
                Overlay.Notice($"Git 邮箱已设置: {_userEmail}", NotificationType.Success);
            }
        }
        catch (Exception ex)
        {
            Overlay.Notice($"应用 Git 用户设置失败: {ex.Message}", NotificationType.Error);
        }
    }

    private async Task ApplyProxySettings()
    {
        try
        {
            if (_isProxyEnabled)
            {
                if (string.IsNullOrWhiteSpace(_proxyAddress))
                {
                    Overlay.Notice("代理地址为空，无法启用代理", NotificationType.Warning);
                    return;
                }

                await ExecuteGitCommand($"config --global http.proxy {_proxyAddress}");
                await ExecuteGitCommand($"config --global https.proxy {_proxyAddress}");

                Data.SettingEntry.GitProxyAddress = _proxyAddress;
                Overlay.Notice($"Git 代理已启用: {_proxyAddress}", NotificationType.Success);
            }
            else
            {
                Data.SettingEntry.GitProxyAddress = _proxyAddress;

                await ExecuteGitCommand("config --global --unset http.proxy");
                await ExecuteGitCommand("config --global --unset https.proxy");

                Overlay.Notice("Git 代理已关闭", NotificationType.Success);
            }
        }
        catch (Exception ex)
        {
            Overlay.Notice($"应用 Git 代理设置失败: {ex.Message}", NotificationType.Error);
        }
    }

    private async Task<string> ExecuteGitCommand(string arguments)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0 || string.IsNullOrWhiteSpace(error)) return output;
            if (!error.Contains("no such section or name"))
            {
                Overlay.Notice($"Git 命令执行警告: {error}", NotificationType.Warning);
            }

            return output;
        }
        catch (Exception ex)
        {
            Overlay.Notice($"执行 Git 命令失败: {ex.Message}", NotificationType.Error);
            return string.Empty;
        }
    }
}