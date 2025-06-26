using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    /// <summary>
    /// Converts string to Visibility - Visible if string is not null/empty, Collapsed otherwise.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public static readonly StringToVisibilityConverter Instance = new StringToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}