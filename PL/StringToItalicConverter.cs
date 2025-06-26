using System;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    /// <summary>
    /// Converter to show italic text when volunteer name is empty
    /// </summary>
    public class StringToItalicConverter : IValueConverter
    {
        public static readonly StringToItalicConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? FontStyles.Italic : FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter to show visibility indicator when count > 0
    /// </summary>
    public class CountToVisibilityConverter : IValueConverter
    {
        public static readonly CountToVisibilityConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}