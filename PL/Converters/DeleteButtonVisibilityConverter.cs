using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;

namespace PL.Converters
{
    // כפתור מחיקה - מוצג רק אם הקריאה פתוחה וללא הקצאות
    public class DeleteButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInList call)
            {
                return ((call.Status == BO.CallStatus.Open|| call.Status == BO.CallStatus.OpenRisk) && call.AssignmentsCount == 0)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // כפתור ביטול הקצאה - מוצג רק אם הקריאה בטיפול ויש לה מתנדב
    public class CancelAssignmentVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInList call)
            {
                return ((call.Status == BO.CallStatus.InProgress|| call.Status == BO.CallStatus.InProgressRisk) && !string.IsNullOrEmpty(call.LastVolunteerName))
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}