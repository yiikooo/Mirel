using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using Mirel.Const;
using Mirel.Module.Ui;

namespace Mirel.Module.IO.Local;

public static class Picker
{
    extension(Control sender)
    {
        public async Task<IReadOnlyList<string>> PickFolderAsync(FolderPickerOpenOptions options)
        {
            var StorageProvider = TopLevel.GetTopLevel(sender).StorageProvider;
            if (Data.SettingEntry.UseFilePicker)
            {
                var res = await StorageProvider.OpenFolderPickerAsync(options);
                return res.Select(x =>
                {
                    try
                    {
                        return x.Path.LocalPath;
                    }
                    catch
                    {
                        return x.Name;
                    }
                }).ToList();
            }

            var text = new TextBox { Watermark = "输入文件夹路径", Text = string.Empty };
            return await Overlay.ShowDialogAsync(options.Title ?? "Mirel", null, text, "确定", "取消",
                    sender: sender) ==
                ContentDialogResult.Primary && !text.Text.IsNullOrWhiteSpace()
                    ? [text.Text]!
                    : [];
        }

        public async Task<IReadOnlyList<string>> PickFileAsync(FilePickerOpenOptions options)
        {
            var StorageProvider = TopLevel.GetTopLevel(sender).StorageProvider;
            if (Data.SettingEntry.UseFilePicker)
                return (await StorageProvider.OpenFilePickerAsync(options)).Select(x => x.Path.LocalPath).ToList();
            var text = new TextBox { Watermark = "输入文件路径", Text = string.Empty };
            return await Overlay.ShowDialogAsync(options.Title ?? "Mirel", null, text, "确定", "取消",
                    sender: sender) ==
                ContentDialogResult.Primary && !text.Text.IsNullOrWhiteSpace()
                    ? [text.Text]!
                    : [];
        }

        public async Task<string> PickSaveFileAsync(FilePickerSaveOptions options)
        {
            var StorageProvider = TopLevel.GetTopLevel(sender).StorageProvider;
            if (Data.SettingEntry.UseFilePicker)
            {
                var saveFilePickerAsync = await StorageProvider.SaveFilePickerAsync(options);
                return saveFilePickerAsync?.Path.LocalPath;
            }

            var text = new TextBox { Watermark = "输入文件路径", Text = string.Empty };
            return await Overlay.ShowDialogAsync(options.Title ?? "Mirel", null, text, "确定", "取消",
                    sender: sender) ==
                ContentDialogResult.Primary && !text.Text.IsNullOrWhiteSpace()
                    ? text.Text
                    : null;
        }
    }
}