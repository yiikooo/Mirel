using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Mirel.Module.Ui;

namespace Mirel.Module.IO.Local;

public abstract class Setter
{
    public static void TryCreateFolder(string path)
    {
        if (Directory.Exists(path)) return;
        var directoryInfo = new DirectoryInfo(path);
        directoryInfo.Create();
    }

    public static void ClearFolder(string folderPath, string[]? ignore = null)
    {
        if (ignore != null && Enumerable.Contains(ignore, folderPath)) return;
        if (!Directory.Exists(folderPath)) return;

        foreach (var file in Directory.GetFiles(folderPath)) File.Delete(file);

        foreach (var dir in Directory.GetDirectories(folderPath))
        {
            ClearFolder(dir, ignore);
            Directory.Delete(dir);
        }
    }

    public static void TryClearFolder(string folderPath, string[]? ignore = null)
    {
        try
        {
            ClearFolder(folderPath, ignore);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public static async Task<bool> CopyFileWithDialog(string source, string target)
    {
        var path = target;
        if (File.Exists(target))
        {
            var cr = await Overlay.ShowDialogAsync($"冲突: {Path.GetFileName(source)}", "文件冲突",
                b_primary: "覆盖",
                b_secondary: "重命名", b_cancel: "取消");
            if (cr == ContentDialogResult.Primary)
            {
                if (source == path) return false;
                File.Copy(source, target, true);
            }
            else if (cr == ContentDialogResult.None)
            {
                return false;
            }
            else
            {
                var textBox = new TextBox
                {
                    FontFamily = (FontFamily)Application.Current.Resources["Font"], TextWrapping = TextWrapping.Wrap,
                    Text = Path.GetFileName(target), HorizontalAlignment = HorizontalAlignment.Stretch, Width = 500
                };
                var cr1 = await Overlay.ShowDialogAsync("重命名", p_content: textBox, b_cancel: "取消",
                    b_primary: "确定");
                if (cr1 != ContentDialogResult.Primary) return false;
                path = Path.Combine(Path.GetDirectoryName(target)!, textBox.Text);
                return await CopyFileWithDialog(source, path);
            }
        }
        else
        {
            if (source == path) return false;
            File.Copy(source, path, true);
        }

        return true;
    }
}