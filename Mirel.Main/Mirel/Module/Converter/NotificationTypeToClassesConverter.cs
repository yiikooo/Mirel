using System;
using System.Globalization;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Converters;

namespace Mirel.Module.Converter;

public class NotificationTypeToClassesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not NotificationType t) return null;
        Avalonia.Controls.Classes classes =
        [
            t switch
            {
                NotificationType.Information => "info",
                NotificationType.Success => "success",
                NotificationType.Warning => "warn",
                NotificationType.Error => "error",
                _ => "info"
            }

        ];
        return classes;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}