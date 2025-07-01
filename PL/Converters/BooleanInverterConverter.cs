using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    /// <summary>
    /// Converter for inverting boolean values (for enabling/disabling controls when simulator is running)
    /// </summary>
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }

    /// <summary>
    /// Converter for simulator button text based on running state
    /// </summary>
    public class SimulatorButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRunning)
                return isRunning ? "STOP SIMULATOR" : "START SIMULATOR";
            return "START SIMULATOR";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}