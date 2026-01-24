using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Aurelio.Public.Classes.Enum;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Downloader;
using Flurl.Http;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Const;
using Mirel.Module.Events;
using Mirel.Module.IO.Http;
using Newtonsoft.Json.Linq;

namespace Mirel.Module.App.Services;

public class Update
{
    private const string GITHUB_API = "https://api.github.com/repos/Yeppioo/Mirel/releases?per_page=1";
    private const string DOWNLOAD_BASE_URL = "https://github.com/Yeppioo/Mirel/releases/download/AutoPublish/";

    public static async Task<UpdateInfo> CheckUpdate()
    {
        var json = await GITHUB_API
            .WithHeader("User-Agent", "Mirel-App")
            .GetStringAsync();

        var latest = JArray.Parse(json)[0];

        var info = new UpdateInfo
        {
            Body = latest["body"].ToString(),
            ReleaseTime = DateTime.Parse(latest["published_at"].ToString()),
            NewVersion = latest["name"].ToString(),
            IsNeedUpdate = latest["name"].ToString() != Data.Instance.Version
        };

        return info;
    }

    public static async Task UpdateApp(Control sender)
    {
        if (Data.DesktopType != DesktopType.Windows) return;
        if (Environment.OSVersion.Version.Major < 10) return;
        var file = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "Mirel.win.x64.installer.exe",
            Architecture.X86 => "Mirel.win.x86.installer.exe",
            Architecture.Arm64 => "Mirel.win.arm64.installer.exe",
            _ => null
        };
        if (file.IsNullOrWhiteSpace()) return;

        var downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 5,
            ParallelDownload = true
        };

        var downloader = new DownloadService(downloadOpt);
        var cts = new CancellationTokenSource();

        var task = Tasking.CreateTask($"更新 App");
        new TaskEntry($"下载: {file}") { TaskState = TaskState.Running }.AddIn(task);
        new TaskEntry($"开始安装: {file}").AddIn(task);
        task.ProgressIsIndeterminate = false;
        task.ButtonAction = () =>
        {
            task.CancelWaitFinish();
            cts.Cancel();
            downloader.CancelAsync();
        };
        task.IsButtonEnable = true;
        task.ButtonText = "取消";
        task.TaskState = TaskState.Running;

        // _ = OpenTaskDrawer(GetHostId(sender));

        downloader.DownloadProgressChanged += (o, e) =>
        {
            var estimateTime =
                (int)Math.Ceiling((e.TotalBytesToReceive - e.ReceivedBytesSize) / e.AverageBytesPerSecondSpeed);
            var timeLeftUnit = "s";

            if (estimateTime >= 60)
            {
                timeLeftUnit = "min";
                estimateTime /= 60;
            }
            else if (estimateTime < 0)
            {
                estimateTime = 0;
            }

            var bytesReceived = e.ReceivedBytesSize.CalcMemoryMensurableUnit();
            var totalBytesToReceive = e.TotalBytesToReceive.CalcMemoryMensurableUnit();
            task.ProgressValue = Math.Round(e.ProgressPercentage, 2);
            task.TopRightInfoText = $"{e.BytesPerSecondSpeed.CalcMemoryMensurableUnit()}/s";
            task.BottomLeftInfoText = $"[{bytesReceived}/{totalBytesToReceive}] {estimateTime} {timeLeftUnit} left";
        };

        downloader.DownloadFileCompleted += (_, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (args.Cancelled)
                {
                    task.CancelFinish();
                }
                else
                {
                    if (args.Error != null)
                    {
                        Logger.Error(args.Error);
                        task.FinishWithError();
                        Ui.Overlay.Notice($"下载失败: {args.Error.Message}");
                    }
                    else
                    {
                        task.FinishWithSuccess();
                        Ui.Overlay.Notice($"下载完成: {file}", NotificationType.Success);
                        task.NextSubTask();

                        if (!await AppEvents.OnAppExiting()) return;
                        var startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory,
                            FileName = Path.Combine(ConfigPath.TempFolderPath, file!)
                        };
                        Process.Start(startInfo);
                        Environment.Exit(0);
                    }
                }
            });
        };

        await downloader.DownloadFileTaskAsync(
            Data.SettingEntry.EnableSpeedUpGithubApi
                ? Data.SettingEntry.GithubSpeedUpApiUrl.Replace("%url%", $"{DOWNLOAD_BASE_URL}{file}")
                : $"{DOWNLOAD_BASE_URL}{file}", Path.Combine
                (ConfigPath.TempFolderPath, file!), cts.Token);
    }

    public static async Task Download(string file, string path, Control sender)
    {
        var downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 5,
            ParallelDownload = true,
            // RequestConfiguration =
            // {
            //     Accept = "*/*",
            //     UserAgent =
            //         "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763",
            //     KeepAlive = true,
            //     Headers =
            //     [
            //         $"Host: {(Data.SettingEntry.EnableSpeedUpGithubApi ? new Uri(Data.SettingEntry.GithubSpeedUpApiUrl).Host : "github.com")}"
            //     ]
            // }
        };

        var downloader = new DownloadService(downloadOpt);

        var cts = new CancellationTokenSource();

        var task = Tasking.CreateTask($"下载: {file}");
        new TaskEntry($"下载: {file}") { TaskState = TaskState.Running }.AddIn(task);
        task.ProgressIsIndeterminate = false;
        task.ButtonAction = () =>
        {
            task.CancelWaitFinish();
            cts.Cancel();
            downloader.CancelAsync();
        };
        task.IsButtonEnable = true;
        task.ButtonText = "取消";
        task.TaskState = TaskState.Running;

        // _ = OpenTaskDrawer(GetHostId(sender));

        downloader.DownloadProgressChanged += (o, e) =>
        {
            var estimateTime =
                (int)Math.Ceiling((e.TotalBytesToReceive - e.ReceivedBytesSize) / e.AverageBytesPerSecondSpeed);
            var timeLeftUnit = "s";

            if (estimateTime >= 60)
            {
                timeLeftUnit = "min";
                estimateTime /= 60;
            }
            else if (estimateTime < 0)
            {
                estimateTime = 0;
            }

            var bytesReceived = e.ReceivedBytesSize.CalcMemoryMensurableUnit();
            var totalBytesToReceive = e.TotalBytesToReceive.CalcMemoryMensurableUnit();
            task.ProgressValue = Math.Round(e.ProgressPercentage, 2);
            task.TopRightInfoText = $"{e.BytesPerSecondSpeed.CalcMemoryMensurableUnit()}/s";
            task.BottomLeftInfoText = $"[{bytesReceived}/{totalBytesToReceive}] {estimateTime} {timeLeftUnit} left";
        };

        downloader.DownloadFileCompleted += (_, args) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (args.Cancelled)
                {
                    task.CancelFinish();
                }
                else
                {
                    if (args.Error != null)
                    {
                        Logger.Error(args.Error);
                        task.FinishWithError();
                        Ui.Overlay.Notice($"下载失败: {args.Error.Message}");
                    }
                    else
                    {
                        task.FinishWithSuccess();
                        Ui.Overlay.Notice($"下载完成: {file}", NotificationType.Success);
                    }
                }
            });
        };

        await downloader.DownloadFileTaskAsync(
            Data.SettingEntry.EnableSpeedUpGithubApi
                ? Data.SettingEntry.GithubSpeedUpApiUrl.Replace("%url%", $"{DOWNLOAD_BASE_URL}{file}")
                : $"{DOWNLOAD_BASE_URL}{file}", path, cts.Token);
    }
}