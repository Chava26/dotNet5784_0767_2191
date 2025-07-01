using System.Windows;
using System.Windows.Controls;
using BlApi;
//using static BO.Enums;

namespace PL
{
    public partial class AddCallWindow : Window
    {
        private static readonly IBl s_bl = Factory.Get();

        public IEnumerable<BO.CallType> CallTypeCollection =>
            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

        public AddCallWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var grid = (Grid)this.Content;

                if (grid.Children.Count < 6)
                {
                    MessageBox.Show("Missing details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var stack1 = (StackPanel)grid.Children[0]; // ??? ÷????
                var stack2 = (StackPanel)grid.Children[1]; // ?????
                var stack3 = (StackPanel)grid.Children[2]; // ?????
                var stack4 = (StackPanel)grid.Children[3]; // ????? ?????
                var stack5 = (StackPanel)grid.Children[4]; // ????? ???

                var callTypeComboBox = (ComboBox)stack1.Children[1];
                var descriptionTextBox = (TextBox)stack2.Children[1];
                var addressTextBox = (TextBox)stack3.Children[1];
                var openingDatePicker = (DatePicker)stack4.Children[1];
                var maxFinishDatePicker = (DatePicker)stack5.Children[1];

                var newCall = new BO.Call
                {
                    CallType = (BO.CallType)callTypeComboBox.SelectedItem,
                    Description = descriptionTextBox.Text,
                    FullAddress = addressTextBox.Text,
                    Latitude = null,
                    Longitude = null,
                    OpenTime = DateTime.Now,
                    MaxEndTime = maxFinishDatePicker.SelectedDate ?? null,
                    Status = BO.CallStatus.Open
                };

                s_bl.Call.AddCall(newCall);
                MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                string fullMessage = $"Error adding call: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullMessage += $"\nInner exception: {ex.InnerException.Message}";
                }
                MessageBox.Show(fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using System.Windows;
//using BlApi;

//namespace PL
//{
//    public partial class AddCallWindow : Window, INotifyPropertyChanged
//    {
//        private static readonly IBl s_bl = Factory.Get();

//        public IEnumerable<BO.CallType> CallTypeCollection =>
//            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

//        // Properties for binding
//        private BO.CallType _selectedCallType;
//        public BO.CallType SelectedCallType
//        {
//            get => _selectedCallType;
//            set
//            {
//                _selectedCallType = value;
//                OnPropertyChanged();
//            }
//        }

//        private string _description = string.Empty;
//        public string Description
//        {
//            get => _description;
//            set
//            {
//                _description = value;
//                OnPropertyChanged();
//            }
//        }

//        private string _fullAddress = string.Empty;
//        public string FullAddress
//        {
//            get => _fullAddress;
//            set
//            {
//                _fullAddress = value;
//                OnPropertyChanged();
//            }
//        }

//        private DateTime _openTime = DateTime.Now;
//        public DateTime OpenTime
//        {
//            get => _openTime;
//            set
//            {
//                _openTime = value;
//                OnPropertyChanged();
//            }
//        }

//        private DateTime? _maxFinishTime;
//        public DateTime? MaxFinishTime
//        {
//            get => _maxFinishTime;
//            set
//            {
//                _maxFinishTime = value;
//                OnPropertyChanged();
//            }
//        }

//        private string _statusMessage = string.Empty;
//        public string StatusMessage
//        {
//            get => _statusMessage;
//            set
//            {
//                _statusMessage = value;
//                OnPropertyChanged();
//            }
//        }

//        public AddCallWindow()
//        {
//            InitializeComponent();
//            DataContext = this;

//            // Set default values
//            if (CallTypeCollection.Any())
//                SelectedCallType = CallTypeCollection.First();
//        }

//        /// <summary>
//        /// Handles adding a new call and implements auto-close functionality
//        /// </summary>
//        private async void AddButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                // Validate required fields
//                if (SelectedCallType == BO.CallType.None)
//                {
//                    MessageBox.Show("נא לבחור סוג קריאה.", "שגיאת קלט",
//                                   MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (string.IsNullOrWhiteSpace(Description))
//                {
//                    MessageBox.Show("נא להקיש תיאור לקריאה.", "שגיאת קלט",
//                                   MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (string.IsNullOrWhiteSpace(FullAddress))
//                {
//                    MessageBox.Show("נא להקיש כתובת.", "שגיאת קלט",
//                                   MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                var newCall = new BO.Call
//                {
//                    CallType = SelectedCallType,
//                    Description = Description.Trim(),
//                    FullAddress = FullAddress.Trim(),
//                    Latitude = null,
//                    Longitude = null,
//                    OpenTime = OpenTime,
//                    MaxEndTime = MaxFinishTime ?? DateTime.Now.AddHours(24),
//                    Status = BO.CallStatus.Open
//                };

//                // Add the call
//                s_bl.Call.AddCall(newCall);

//                // Show success message briefly
//                StatusMessage = "הקריאה נוספה בהצלחה! ??";

//                // Show success dialog
//                MessageBox.Show("הקריאה נוספה בהצלחה!\nהחלון ייסגר אוטומטית.", "הצלחה",
//                               MessageBoxButton.OK, MessageBoxImage.Information);

//                // Auto-close after brief delay to allow user to see success message
//                await Task.Delay(500);

//                // Close the window - this will trigger automatic refresh in parent window via Observer pattern
//                DialogResult = true;
//                Close();
//            }
//            catch (Exception ex)
//            {
//                string fullMessage = $"שגיאה בהוספת קריאה: {ex.Message}";
//                if (ex.InnerException != null)
//                {
//                    fullMessage += $"\nפרטים נוספים: {ex.InnerException.Message}";
//                }

//                StatusMessage = "שגיאה בהוספת קריאה";
//                MessageBox.Show(fullMessage, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// Clears all input fields to allow adding another call (not used anymore due to auto-close)
//        /// </summary>
//        private void ClearAllFields()
//        {
//            // Reset to first call type
//            if (CallTypeCollection.Any())
//                SelectedCallType = CallTypeCollection.First();

//            Description = string.Empty;
//            FullAddress = string.Empty;
//            OpenTime = DateTime.Now;
//            MaxFinishTime = null;
//            StatusMessage = string.Empty;
//        }

//        /// <summary>
//        /// Closes the window without saving
//        /// </summary>
//        private void CancelButton_Click(object sender, RoutedEventArgs e)
//        {
//            DialogResult = false;
//            Close();
//        }

//        // INotifyPropertyChanged implementation
//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged([CallerMemberName] string name = null)
//            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//    }
//}