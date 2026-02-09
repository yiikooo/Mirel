using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;

namespace Mirel.Module.Converter;

public class IsSingletonPageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is PageRegistration pageReg && typeof(IMirelSingletonTabPage).IsAssignableFrom(pageReg.PageType);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}