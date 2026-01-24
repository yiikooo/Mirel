using System;
using System.Globalization;
using Aurelio.Public.Classes.Enum;
using Avalonia.Data.Converters;

namespace Mirel.Module.Converter;

public class TaskStateToTipConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TaskState state)
            return state switch
            {
                TaskState.Waiting => "等待中",
                TaskState.Running => "运行中",
                TaskState.Paused => "已暂停",
                TaskState.Error => "错误",
                TaskState.Canceled => "已取消",
                TaskState.Canceling => "取消中",
                TaskState.Finished => "已完成",
                _ => state.ToString()
            };
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}