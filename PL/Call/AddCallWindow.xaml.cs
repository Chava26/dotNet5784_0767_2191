using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using BlApi;

namespace PL
{
    public partial class AddCallWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();

        public IEnumerable<BO.CallType> CallTypeCollection =>
            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>()
                .Where(ct => ct != BO.CallType.None); // Remove "None" from selection

        // Basic call properties
        private BO.CallType _selectedCallType = BO.CallType.General;
        public BO.CallType SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged();
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _fullAddress = string.Empty;
        public string FullAddress
        {
            get => _fullAddress;
            set
            {
                _fullAddress = value;
                OnPropertyChanged();
            }
        }

        private DateTime _openTime = DateTime.Now;
        public DateTime OpenTime
        {
            get => _openTime;
            set
            {
                _openTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _maxFinishTime;
        public DateTime? MaxFinishTime
        {
            get => _maxFinishTime;
            set
            {
                _maxFinishTime = value;
                OnPropertyChanged();
            }
        }

        // Vehicle information properties
        private string _vehicleModel = string.Empty;
        public string VehicleModel
        {
            get => _vehicleModel;
            set
            {
                _vehicleModel = value;
                OnPropertyChanged();
            }
        }

        private string _licensePlate = string.Empty;
        public string LicensePlate
        {
            get => _licensePlate;
            set
            {
                _licensePlate = value;
                OnPropertyChanged();
            }
        }

        // Customer information properties
        private string _customerName = string.Empty;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged();
            }
        }

        private string _customerPhone = string.Empty;
        public string CustomerPhone
        {
            get => _customerPhone;
            set
            {
                _customerPhone = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public AddCallWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Set default values
            SelectedCallType = BO.CallType.General;
            MaxFinishTime = DateTime.Now.AddHours(4); // Default 4 hours for completion
        }

        /// <summary>
        /// Handles adding a new automotive service call with auto-close functionality
        /// </summary>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (SelectedCallType == BO.CallType.None)
                {
                    ShowErrorMessage("Please select a call type.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    ShowErrorMessage("Please provide a detailed description of the issue.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(FullAddress))
                {
                    ShowErrorMessage("Please provide the full address of the vehicle location.");
                    return;
                }

                // Build comprehensive description including vehicle and customer info
                string fullDescription = BuildFullDescription();

                var newCall = new BO.Call
                {
                    CallType = SelectedCallType,
                    Description = fullDescription,
                    FullAddress = FullAddress.Trim(),
                    Latitude = null, // Can be enhanced with GPS coordinates
                    Longitude = null,
                    OpenTime = OpenTime,
                    MaxEndTime = MaxFinishTime ?? DateTime.Now.AddHours(4),
                    Status = DetermineInitialStatus()
                };

                // Add the call to the system
                s_bl.Call.AddCall(newCall);

                // Show success message
                StatusMessage = "Service call successfully added to the system!";

                // Show success dialog with call details
                string successMessage = $"Service call created successfully!\n" +
                                      $"Issue Type: {SelectedCallType}\n" +
                                      $"Location: {FullAddress}\n" +
                                      $"The nearest technician will be notified.";

                MessageBox.Show(successMessage, "Call Created Successfully",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                // Auto-close after brief delay
                await Task.Delay(300);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                string fullMessage = $"Error creating service call: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullMessage += $"\nAdditional details: {ex.InnerException.Message}";
                }

                StatusMessage = "Error creating service call";
                MessageBox.Show(fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Builds a comprehensive description including all relevant information
        /// </summary>
        private string BuildFullDescription()
        {
            var descriptionParts = new List<string>
            {
                $"Issue Description: {Description.Trim()}"
            };

            if (!string.IsNullOrWhiteSpace(VehicleModel))
                descriptionParts.Add($"Vehicle Model: {VehicleModel.Trim()}");

            if (!string.IsNullOrWhiteSpace(LicensePlate))
                descriptionParts.Add($"License Plate: {LicensePlate.Trim()}");

            if (!string.IsNullOrWhiteSpace(CustomerName))
                descriptionParts.Add($"Customer Name: {CustomerName.Trim()}");

            if (!string.IsNullOrWhiteSpace(CustomerPhone))
                descriptionParts.Add($"Contact Phone: {CustomerPhone.Trim()}");

            return string.Join("\n", descriptionParts);
        }

        /// <summary>
        /// Determines initial status based on call type severity
        /// </summary>
        private BO.CallStatus DetermineInitialStatus()
        {
            // Critical calls that need immediate attention
            var criticalTypes = new[]
            {
                BO.CallType.EngineFailure,
                BO.CallType.BrakeFailure,
                BO.CallType.Overheating,
                BO.CallType.TransmissionIssue
            };

            if (criticalTypes.Contains(SelectedCallType))
            {
                return BO.CallStatus.OpenRisk; // High priority
            }

            return BO.CallStatus.Open; // Normal priority
        }

        /// <summary>
        /// Shows error message in a user-friendly way
        /// </summary>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Input Error",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Clears all input fields (not used due to auto-close, but kept for future use)
        /// </summary>
        private void ClearAllFields()
        {
            SelectedCallType = BO.CallType.General;
            Description = string.Empty;
            FullAddress = string.Empty;
            VehicleModel = string.Empty;
            LicensePlate = string.Empty;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            OpenTime = DateTime.Now;
            MaxFinishTime = DateTime.Now.AddHours(4);
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Closes the window without saving
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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