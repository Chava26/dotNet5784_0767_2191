using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BlApi;
using BO;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();
        private int _callId;

        // DispatcherOperation object for the RefreshCall observer method
        private volatile DispatcherOperation? _refreshCallOperation = null;

        public BO.Call Call { get; set; }
        public IEnumerable<BO.CallType> CallTypes => Enum.GetValues(typeof(BO.CallType)).Cast<CallType>();

        // Properties for UI logic
        public bool IsCallTypeEditable => Call.Status == BO.CallStatus.OpenRisk || Call.Status == BO.CallStatus.Open;
        public bool IsDescriptionReadOnly => !(Call.Status == BO.CallStatus.OpenRisk || Call.Status == BO.CallStatus.Open);
        public bool IsAddressReadOnly => !(Call.Status == BO.CallStatus.OpenRisk || Call.Status == BO.CallStatus.Open);
        public bool IsMaxFinishTimeReadOnly => !(Call.Status == BO.CallStatus.OpenRisk || Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.InProgressRisk || Call.Status == BO.CallStatus.InProgress);
        public bool IsUpdateEnabled => Call.Status == BO.CallStatus.Open || Call.Status == BO.CallStatus.OpenRisk || Call.Status == BO.CallStatus.InProgressRisk || Call.Status == BO.CallStatus.InProgress;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public CallWindow(int callId)
        {
            InitializeComponent();
            _callId = callId;
            Call = s_bl.Call.GetCallDetails(callId);
            DataContext = this;

            // Add observer for the call
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        /// <summary>
        /// Subscribes to call updates when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Call != null)
            {
                s_bl.Call.AddObserver(Call.Id, RefreshCall);
            }
            s_bl.Admin.AddConfigObserver(RefreshCall);

        }

        /// <summary>
        /// Unsubscribes from call updates when the window is closed.
        /// </summary>
        private void Window_Closed(object? sender, EventArgs e)
        {
            if (Call != null)
            {
                s_bl.Call.RemoveObserver(Call.Id, RefreshCall);
            }
            s_bl.Admin.RemoveConfigObserver(RefreshCall);
        }

        /// <summary>
        /// Refreshes the call details from the business logic layer.
        /// Updated for stage 7 with DispatcherOperation implementation.
        /// </summary>
        private void RefreshCall()
        {
            if (_refreshCallOperation is null || _refreshCallOperation.Status == DispatcherOperationStatus.Completed)
                _refreshCallOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (Call == null) return;

                    int id = Call.Id;
                    Call = s_bl.Call.GetCallDetails(id);

                    // Trigger PropertyChanged for all status-dependent properties
                    OnPropertyChanged(nameof(IsCallTypeEditable));
                    OnPropertyChanged(nameof(IsDescriptionReadOnly));
                    OnPropertyChanged(nameof(IsAddressReadOnly));
                    OnPropertyChanged(nameof(IsMaxFinishTimeReadOnly));
                    OnPropertyChanged(nameof(IsUpdateEnabled));
                });
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic format validations (e.g., description not empty)
                if (string.IsNullOrWhiteSpace(Call.Description))
                {
                    MessageBox.Show("יש להזין תיאור לקריאה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Send update to BL
                s_bl.Call.UpdateCallDetails(Call);
                MessageBox.Show("הקריאה עודכנה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                string fullMessage = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullMessage += $"\nInner exception: {ex.InnerException.Message}";
                }
                MessageBox.Show("שגיאה בעדכון הקריאה: " + fullMessage, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}