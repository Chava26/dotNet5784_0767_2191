using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BlApi;
using BO;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();

        public BO.Call Call { get; set; }
        public IEnumerable<BO.CallType> CallTypes => Enum.GetValues(typeof(BO.CallType)).Cast<CallType>();

        // Properties for UI logic
        public bool IsCallTypeEditable => Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open;
        public bool IsDescriptionReadOnly => !(Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open);
        public bool IsAddressReadOnly => !(Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open);
        public bool IsMaxFinishTimeReadOnly => !(Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open);
        public bool IsUpdateEnabled => Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.Open;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public CallWindow(int callId)
        {
            InitializeComponent();
            Call = s_bl.Call.GetCallDetails(callId);
            DataContext = this;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקות פורמט בסיסיות (לדוג' תיאור לא ריק)
                if (string.IsNullOrWhiteSpace(Call.Description))
                {
                    MessageBox.Show("יש להזין תיאור לקריאה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // שלח עדכון ל-BL
                s_bl.Call.UpdateCallDetails(Call);
                MessageBox.Show("הקריאה עודכנה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון הקריאה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}