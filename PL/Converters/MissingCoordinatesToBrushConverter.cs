using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BO;

namespace PL.Converters
{
    public class MissingCoordinatesToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.Call call)
            {
                if (call.Latitude == null || call.Longitude == null)
                    return Brushes.Red;
            }

            return Brushes.Transparent; // רקע רגיל אם יש קואורדינטות
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}



