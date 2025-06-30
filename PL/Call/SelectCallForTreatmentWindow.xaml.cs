
//using BO;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL.Call
//{
//    public partial class SelectCallForTreatmentWindow : Window, INotifyPropertyChanged
//    {
//        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        private ObservableCollection<OpenCallInList> _openCalls;
//        public ObservableCollection<OpenCallInList> OpenCalls
//        {
//            get => _openCalls;
//            set
//            {
//                _openCalls = value;
//                OnPropertyChanged(nameof(OpenCalls));
//            }
//        }

//        private BO.Volunteer _currentVolunteer;
//        public BO.Volunteer CurrentVolunteer
//        {
//            get => _currentVolunteer;
//            set
//            {
//                _currentVolunteer = value;
//                OnPropertyChanged(nameof(CurrentVolunteer));
//            }
//        }

//        public SelectCallForTreatmentWindow()
//        {
//            InitializeComponent();
//            DataContext = this;
//        }

//        /// <summary>
//        /// Registers observers and loads initial data on window load.
//        /// </summary>
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                // Load current volunteer
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(App.CurrentUserId);

//                // Register observers for automatic updates
//                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, RefreshVolunteer);
//                s_bl.Call.AddObserver(RefreshOpenCalls);

//                // Load initial data and register observers for each call
//                RefreshOpenCalls();
//                RegisterCallObservers();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
//                               MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// Unregisters observers on window close.
//        /// </summary>
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            if (CurrentVolunteer != null)
//            {
//                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteer);
//            }
//            s_bl.Call.RemoveObserver(RefreshOpenCalls);
//            UnregisterCallObservers();
//            Close();
//        }

//        /// <summary>
//        /// Registers observers for individual calls to track status changes.
//        /// </summary>
//        private void RegisterCallObservers()
//        {
//            if (OpenCalls != null)
//            {
//                foreach (var call in OpenCalls)
//                {
//                    s_bl.Call.AddObserver(call.Id, RefreshSpecificCall);
//                }
//            }
//        }

//        /// <summary>
//        /// Unregisters observers for individual calls.
//        /// </summary>
//        private void UnregisterCallObservers()
//        {
//            if (OpenCalls != null)
//            {
//                foreach (var call in OpenCalls)
//                {
//                    s_bl.Call.RemoveObserver(call.Id, RefreshSpecificCall);
//                }
//            }
//        }

//        /// <summary>
//        /// Refreshes volunteer data when it changes in the business layer.
//        /// </summary>
//        private void RefreshVolunteer()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                if (CurrentVolunteer != null)
//                {
//                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
//                    RefreshOpenCalls(); // Refresh calls when volunteer data changes
//                }
//            });
//        }

//        /// <summary>
//        /// Refreshes open calls list when data changes in the business layer.
//        /// </summary>
//        private void RefreshOpenCalls()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                try
//                {
//                    // Unregister observers for old calls
//                    UnregisterCallObservers();

//                    var filteredCalls = GetFilteredOpenCalls();
//                    OpenCalls = new ObservableCollection<OpenCallInList>(filteredCalls);

//                    // Register observers for new calls
//                    RegisterCallObservers();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error refreshing open calls: {ex.Message}", "Error",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            });
//        }

//        /// <summary>
//        /// Refreshes a specific call when its data changes in the business layer.
//        /// </summary>
//        private void RefreshSpecificCall()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                try
//                {
//                    // Refresh the entire list to handle status changes
//                    // (e.g., if a call changes from Open to InProgress, it should be removed)
//                    RefreshOpenCalls();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error refreshing specific call: {ex.Message}", "Error",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            });
//        }

//        /// <summary>
//        /// Gets filtered open calls for the current volunteer within their maximum distance.
//        /// </summary>
//        /// <param name="callType">Optional call type filter</param>
//        /// <param name="sortField">Optional sort field</param>
//        /// <returns>Filtered list of open calls</returns>
//        private IEnumerable<BO.OpenCallInList> GetFilteredOpenCalls(BO.CallType? callType = null, BO.CallField? sortField = null)
//        {
//            try
//            {
//                if (CurrentVolunteer == null) return Enumerable.Empty<BO.OpenCallInList>();

//                // Use the BL method to get open calls for volunteer
//                var openCalls = s_bl.Call.GetOpenCallsForVolunteer(CurrentVolunteer.Id, callType, sortField);

//                // Filter by distance
//                return openCalls.Where(call => call.DistanceFromVolunteer <= CurrentVolunteer.MaxDistanceForTask);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error retrieving filtered open calls for volunteer {CurrentVolunteer?.FullName}: {ex.Message}",
//                               "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                return Enumerable.Empty<BO.OpenCallInList>();
//            }
//        }

//        /// <summary>
//        /// Handles call selection and assignment to volunteer.
//        /// </summary>
//        private void OnSelectCall(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button button && button.DataContext is OpenCallInList selectedCall)
//            {
//                try
//                {
//                    // Assign call to volunteer using business layer
//                    s_bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, selectedCall.Id);

//                    MessageBox.Show($"Call {selectedCall.Id} has been assigned to you.", "Success",
//                                   MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close();
//                    // List will refresh automatically via observers
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error assigning call: {ex.Message}", "Error",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        /// <summary>
//        /// Handles volunteer address update.
//        /// </summary>
//        private void OnUpdateAddress(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var newAddress = ShowAddressInputDialog(CurrentVolunteer?.Address ?? "");
//                if (!string.IsNullOrWhiteSpace(newAddress) && newAddress != CurrentVolunteer?.Address)
//                {
//                    CurrentVolunteer!.Address = newAddress;
//                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);

//                    MessageBox.Show("Address updated successfully.", "Success",
//                                   MessageBoxButton.OK, MessageBoxImage.Information);

//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error updating address: {ex.Message}", "Error",
//                               MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// Shows address input dialog and returns new address.
//        /// </summary>
//        /// <param name="currentAddress">Current volunteer address</param>
//        /// <returns>New address or null if cancelled</returns>
//        private string ShowAddressInputDialog(string currentAddress)
//        {
//            var inputDialog = new Window
//            {
//                Title = "Update Address",
//                Width = 300,
//                Height = 150,
//                WindowStartupLocation = WindowStartupLocation.CenterScreen,
//                Owner = this
//            };

//            var stackPanel = new StackPanel { Margin = new Thickness(10) };
//            var textBox = new TextBox { Text = currentAddress, Margin = new Thickness(0, 0, 0, 10) };
//            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
//            var okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(5) };
//            var cancelButton = new Button { Content = "Cancel", Width = 75, Margin = new Thickness(5) };

//            buttonPanel.Children.Add(okButton);
//            buttonPanel.Children.Add(cancelButton);
//            stackPanel.Children.Add(new TextBlock { Text = "Enter new address:", Margin = new Thickness(0, 0, 0, 5) });
//            stackPanel.Children.Add(textBox);
//            stackPanel.Children.Add(buttonPanel);
//            inputDialog.Content = stackPanel;

//            string newAddress = null;
//            bool dialogResult = false;

//            okButton.Click += (s, e) =>
//            {
//                newAddress = textBox.Text;
//                dialogResult = true;
//                inputDialog.Close();
//            };

//            cancelButton.Click += (s, e) =>
//            {
//                dialogResult = false;
//                inputDialog.Close();
//            };

//            inputDialog.ShowDialog();
//            return dialogResult ? newAddress : null;
//        }

//        /// <summary>
//        /// Closes the window.
//        /// </summary>
//        private void OnClose(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        // INotifyPropertyChanged implementation
//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
//using BO;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;

//namespace PL.Call
//{
//    public partial class SelectCallForTreatmentWindow : Window, INotifyPropertyChanged
//    {
//        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        private ObservableCollection<OpenCallInList> _openCalls;
//        private OpenCallInList _selectedCall;
//        private BO.Volunteer _currentVolunteer;
//        private BO.CallType? _selectedCallType;
//        private BO.CallField? _selectedSortField;
//        private string _callDescription = string.Empty;

//        public ObservableCollection<OpenCallInList> OpenCalls
//        {
//            get => _openCalls;
//            set
//            {
//                _openCalls = value;
//                OnPropertyChanged();
//            }
//        }

//        public OpenCallInList SelectedCall
//        {
//            get => _selectedCall;
//            set
//            {
//                _selectedCall = value;
//                OnPropertyChanged();
//                UpdateCallDescription();
//            }
//        }

//        public BO.Volunteer CurrentVolunteer
//        {
//            get => _currentVolunteer;
//            set
//            {
//                _currentVolunteer = value;
//                OnPropertyChanged();
//            }
//        }

//        public BO.CallType? SelectedCallType
//        {
//            get => _selectedCallType;
//            set
//            {
//                _selectedCallType = value;
//                OnPropertyChanged();
//                RefreshOpenCalls();
//            }
//        }

//        public BO.CallField? SelectedSortField
//        {
//            get => _selectedSortField;
//            set
//            {
//                _selectedSortField = value;
//                OnPropertyChanged();
//                RefreshOpenCalls();
//            }
//        }

//        public string CallDescription
//        {
//            get => _callDescription;
//            set
//            {
//                _callDescription = value;
//                OnPropertyChanged();
//            }
//        }

//        // Enum collections for filtering and sorting
//        public IEnumerable<BO.CallType> CallTypes =>
//            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

//        public IEnumerable<BO.CallField> SortFields =>
//            Enum.GetValues(typeof(BO.CallField)).Cast<BO.CallField>();

//        // Properties for UI state
//        public bool CanSelectCall => CurrentVolunteer?.callInProgress == null && CurrentVolunteer?.IsActive == true;
//        public bool HasSelectedCall => SelectedCall != null;

//        public SelectCallForTreatmentWindow()
//        {
//            InitializeComponent();
//            DataContext = this;
//        }

//        /// <summary>
//        /// Registers observers and loads initial data on window load.
//        /// </summary>
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                // Load current volunteer
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(App.CurrentUserId);

//                // Check if volunteer can select calls
//                if (!CanSelectCall)
//                {
//                    string message = CurrentVolunteer?.IsActive != true
//                        ? "לא ניתן לבחור קריאות כאשר המתנדב לא פעיל."
//                        : "לא ניתן לבחור קריאה חדשה בזמן טיפול בקריאה קיימת.";

//                    MessageBox.Show(message, "לא ניתן לבחור קריאה",
//                                   MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close();
//                    return;
//                }

//                // Register observers for automatic updates
//                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, RefreshVolunteer);
//                s_bl.Call.AddObserver(RefreshOpenCalls);

//                // Load initial data and register observers for each call
//                RefreshOpenCalls();
//                RegisterCallObservers();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בטעינת הנתונים: {ex.Message}", "שגיאה",
//                               MessageBoxButton.OK, MessageBoxImage.Error);
//                Close();
//            }
//        }

//        /// <summary>
//        /// Unregisters observers on window close.
//        /// </summary>
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            if (CurrentVolunteer != null)
//            {
//                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteer);
//            }
//            s_bl.Call.RemoveObserver(RefreshOpenCalls);
//            UnregisterCallObservers();
//        }

//        /// <summary>
//        /// Registers observers for individual calls to track status changes.
//        /// </summary>
//        private void RegisterCallObservers()
//        {
//            if (OpenCalls != null)
//            {
//                foreach (var call in OpenCalls)
//                {
//                    s_bl.Call.AddObserver(call.Id, RefreshSpecificCall);
//                }
//            }
//        }

//        /// <summary>
//        /// Unregisters observers for individual calls.
//        /// </summary>
//        private void UnregisterCallObservers()
//        {
//            if (OpenCalls != null)
//            {
//                foreach (var call in OpenCalls)
//                {
//                    s_bl.Call.RemoveObserver(call.Id, RefreshSpecificCall);
//                }
//            }
//        }

//        /// <summary>
//        /// Refreshes volunteer data when it changes in the business layer.
//        /// </summary>
//        private void RefreshVolunteer()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                if (CurrentVolunteer != null)
//                {
//                    var oldVolunteer = CurrentVolunteer;
//                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);

//                    // If volunteer can no longer select calls, close window
//                    if (!CanSelectCall)
//                    {
//                        MessageBox.Show("החלון ייסגר מכיוון שלא ניתן עוד לבחור קריאות.", "מידע",
//                                       MessageBoxButton.OK, MessageBoxImage.Information);
//                        Close();
//                        return;
//                    }

//                    // If volunteer address changed, refresh calls
//                    if (oldVolunteer.Address != CurrentVolunteer.Address ||
//                        oldVolunteer.Latitude != CurrentVolunteer.Latitude ||
//                        oldVolunteer.Longitude != CurrentVolunteer.Longitude)
//                    {
//                        RefreshOpenCalls();
//                    }

//                    // Update UI state properties
//                    OnPropertyChanged(nameof(CanSelectCall));
//                }
//            });
//        }

//        /// <summary>
//        /// Refreshes open calls list when data changes in the business layer.
//        /// </summary>
//        private void RefreshOpenCalls()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                try
//                {
//                    // Unregister observers for old calls
//                    UnregisterCallObservers();

//                    var filteredCalls = GetFilteredOpenCalls(SelectedCallType, SelectedSortField);
//                    OpenCalls = new ObservableCollection<OpenCallInList>(filteredCalls);

//                    // Clear selection if selected call is no longer available
//                    if (SelectedCall != null && !OpenCalls.Any(c => c.Id == SelectedCall.Id))
//                    {
//                        SelectedCall = null;
//                    }

//                    // Register observers for new calls
//                    RegisterCallObservers();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה ברענון רשימת הקריאות הפתוחות: {ex.Message}", "שגיאה",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            });
//        }

//        /// <summary>
//        /// Refreshes a specific call when its data changes in the business layer.
//        /// </summary>
//        private void RefreshSpecificCall()
//        {
//            Dispatcher.Invoke(() =>
//            {
//                try
//                {
//                    // Refresh the entire list to handle status changes
//                    // (e.g., if a call changes from Open to InProgress, it should be removed)
//                    RefreshOpenCalls();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה ברענון קריאה ספציפית: {ex.Message}", "שגיאה",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            });
//        }

//        /// <summary>
//        /// Gets filtered open calls for the current volunteer within their maximum distance.
//        /// Only shows Open or OpenAtRisk calls.
//        /// </summary>
//        /// <param name="callType">Optional call type filter</param>
//        /// <param name="sortField">Optional sort field</param>
//        /// <returns>Filtered list of open calls</returns>
//        private IEnumerable<BO.OpenCallInList> GetFilteredOpenCalls(BO.CallType? callType = null, BO.CallField? sortField = null)
//        {
//            try
//            {                 
//                if (CurrentVolunteer == null) return Enumerable.Empty<BO.OpenCallInList>();

//            // Use the BL method to get open calls for volunteer
//            var openCalls = s_bl.Call.GetOpenCallsForVolunteer(CurrentVolunteer.Id, callType, sortField);

//            // Filter by distance
//            return openCalls.Where(call => call.DistanceFromVolunteer <= CurrentVolunteer.MaxDistanceForTask);
//        }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בקבלת רשימת קריאות פתוחות עבור המתנדב {CurrentVolunteer?.FullName}: {ex.Message}",
//                               "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//                return Enumerable.Empty<BO.OpenCallInList>();
//            }
//        }

//        /// <summary>
//        /// Updates call description when a call is selected
//        /// </summary>
//        private void UpdateCallDescription()
//        {
//            if (SelectedCall != null)
//            {
//                try
//                {
//                    // Get full call details to access the description
//                    var fullCall = s_bl.Call.GetCallDetails(SelectedCall.Id);
//                    CallDescription = fullCall?.Description ?? "אין תיאור זמין";
//                }
//                catch (Exception ex)
//                {
//                    CallDescription = $"שגיאה בקבלת תיאור הקריאה: {ex.Message}";
//                }
//            }
//            else
//            {
//                CallDescription = "בחר קריאה כדי לראות את התיאור המפורט";
//            }
//        }

//        /// <summary>
//        /// Handles DataGrid selection changed event
//        /// </summary>
//        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is OpenCallInList selectedCall)
//            {
//                SelectedCall = selectedCall;
//            }
//        }

//        /// <summary>
//        /// Handles call selection and assignment to volunteer.
//        /// </summary>
//        private void OnSelectCall(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button button && button.DataContext is OpenCallInList selectedCall)
//            {
//                try
//                {
//                    if (!CanSelectCall)
//                    {
//                        MessageBox.Show("לא ניתן לבחור קריאה כרגע.", "שגיאה",
//                                       MessageBoxButton.OK, MessageBoxImage.Warning);
//                        return;
//                    }

//                    var result = MessageBox.Show(
//                        $"האם אתה בטוח שברצונך לבחור לטפל בקריאה מספר {selectedCall.Id}?",
//                        "אישור בחירת קריאה",
//                        MessageBoxButton.YesNo,
//                        MessageBoxImage.Question);

//                    if (result == MessageBoxResult.Yes)
//                    {
//                        // Assign call to volunteer using business layer
//                        s_bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, selectedCall.Id);

//                        MessageBox.Show($"קריאה מספר {selectedCall.Id} הוקצתה לך בהצלחה.", "הצלחה",
//                                       MessageBoxButton.OK, MessageBoxImage.Information);
//                        Close();
//                        // List will refresh automatically via observers
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה בהקצאת הקריאה: {ex.Message}", "שגיאה",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        /// <summary>
//        /// Handles volunteer address update.
//        /// </summary>
//        private void OnUpdateAddress(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var newAddress = ShowAddressInputDialog(CurrentVolunteer?.Address ?? "");
//                if (!string.IsNullOrWhiteSpace(newAddress) && newAddress != CurrentVolunteer?.Address)
//                {
//                    CurrentVolunteer!.Address = newAddress;
//                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);

//                    MessageBox.Show("הכתובת עודכנה בהצלחה.", "הצלחה",
//                                   MessageBoxButton.OK, MessageBoxImage.Information);

//                    // Volunteer and calls will refresh automatically via observers
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בעדכון הכתובת: {ex.Message}", "שגיאה",
//                               MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// Shows address input dialog and returns new address.
//        /// </summary>
//        /// <param name="currentAddress">Current volunteer address</param>
//        /// <returns>New address or null if cancelled</returns>
//        private string ShowAddressInputDialog(string currentAddress)
//        {
//            var inputDialog = new Window
//            {
//                Title = "עדכון כתובת",
//                Width = 400,
//                Height = 200,
//                WindowStartupLocation = WindowStartupLocation.CenterScreen,
//                Owner = this,
//                FlowDirection = FlowDirection.RightToLeft
//            };

//            var stackPanel = new StackPanel { Margin = new Thickness(10) };
//            var textBox = new TextBox { Text = currentAddress, Margin = new Thickness(0, 0, 0, 10), Height = 25 };
//            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
//            var okButton = new Button { Content = "אישור", Width = 75, Margin = new Thickness(5) };
//            var cancelButton = new Button { Content = "ביטול", Width = 75, Margin = new Thickness(5) };

//            buttonPanel.Children.Add(okButton);
//            buttonPanel.Children.Add(cancelButton);
//            stackPanel.Children.Add(new TextBlock { Text = "הזן כתובת חדשה:", Margin = new Thickness(0, 0, 0, 5), FontSize = 14 });
//            stackPanel.Children.Add(textBox);
//            stackPanel.Children.Add(buttonPanel);
//            inputDialog.Content = stackPanel;

//            string newAddress = null;
//            bool dialogResult = false;

//            okButton.Click += (s, e) =>
//            {
//                newAddress = textBox.Text.Trim();
//                if (string.IsNullOrWhiteSpace(newAddress))
//                {
//                    MessageBox.Show("אנא הזן כתובת תקינה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }
//                dialogResult = true;
//                inputDialog.Close();
//            };

//            cancelButton.Click += (s, e) =>
//            {
//                dialogResult = false;
//                inputDialog.Close();
//            };

//            textBox.KeyDown += (s, e) =>
//            {
//                if (e.Key == System.Windows.Input.Key.Enter)
//                {
//                    okButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
//                }
//            };

//            inputDialog.ShowDialog();
//            return dialogResult ? newAddress : null;
//        }

//        /// <summary>
//        /// Clears all filters
//        /// </summary>
//        private void OnClearFilters(object sender, RoutedEventArgs e)
//        {
//            SelectedCallType = null;
//            SelectedSortField = null;
//        }

//        /// <summary>
//        /// Closes the window.
//        /// </summary>
//        private void OnClose(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        // INotifyPropertyChanged implementation
//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class SelectCallForTreatmentWindow : Window, INotifyPropertyChanged
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private ObservableCollection<OpenCallInList> _openCalls;
        private string _callDescription = "בחר קריאה כדי לראות את התיאור המפורט";

        public ObservableCollection<OpenCallInList> OpenCalls
        {
            get => _openCalls;
            set
            {
                _openCalls = value;
                OnPropertyChanged(nameof(OpenCalls));
            }
        }

        private BO.Volunteer _currentVolunteer;
        public BO.Volunteer CurrentVolunteer
        {
            get => _currentVolunteer;
            set
            {
                _currentVolunteer = value;
                OnPropertyChanged(nameof(CurrentVolunteer));
            }
        }

        public string CallDescription
        {
            get => _callDescription;
            set
            {
                _callDescription = value;
                OnPropertyChanged(nameof(CallDescription));
            }
        }

        public SelectCallForTreatmentWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Registers observers and loads initial data on window load.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load current volunteer
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(App.CurrentUserId);

                // Register observers for automatic updates
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, RefreshVolunteer);
                s_bl.Call.AddObserver(RefreshOpenCalls);

                // Load initial data and register observers for each call
                RefreshOpenCalls();
                RegisterCallObservers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Unregisters observers on window close.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer != null)
            {
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteer);
            }
            s_bl.Call.RemoveObserver(RefreshOpenCalls);
            UnregisterCallObservers();
            Close();
        }

        /// <summary>
        /// Registers observers for individual calls to track status changes.
        /// </summary>
        private void RegisterCallObservers()
        {
            if (OpenCalls != null)
            {
                foreach (var call in OpenCalls)
                {
                    s_bl.Call.AddObserver(call.Id, RefreshSpecificCall);
                }
            }
        }

        /// <summary>
        /// Unregisters observers for individual calls.
        /// </summary>
        private void UnregisterCallObservers()
        {
            if (OpenCalls != null)
            {
                foreach (var call in OpenCalls)
                {
                    s_bl.Call.RemoveObserver(call.Id, RefreshSpecificCall);
                }
            }
        }

        /// <summary>
        /// Refreshes volunteer data when it changes in the business layer.
        /// </summary>
        private void RefreshVolunteer()
        {
            Dispatcher.Invoke(() =>
            {
                if (CurrentVolunteer != null)
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                    RefreshOpenCalls(); // Refresh calls when volunteer data changes
                }
            });
        }

        /// <summary>
        /// Refreshes open calls list when data changes in the business layer.
        /// </summary>
        private void RefreshOpenCalls()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    // Unregister observers for old calls
                    UnregisterCallObservers();

                    var filteredCalls = GetFilteredOpenCalls();
                    OpenCalls = new ObservableCollection<OpenCallInList>(filteredCalls);

                    // Register observers for new calls
                    RegisterCallObservers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing open calls: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Refreshes a specific call when its data changes in the business layer.
        /// </summary>
        private void RefreshSpecificCall()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    // Refresh the entire list to handle status changes
                    // (e.g., if a call changes from Open to InProgress, it should be removed)
                    RefreshOpenCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing specific call: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Gets filtered open calls for the current volunteer within their maximum distance.
        /// </summary>
        /// <param name="callType">Optional call type filter</param>
        /// <param name="sortField">Optional sort field</param>
        /// <returns>Filtered list of open calls</returns>
        private IEnumerable<BO.OpenCallInList> GetFilteredOpenCalls(BO.CallType? callType = null, BO.CallField? sortField = null)
        {
            try
            {
                if (CurrentVolunteer == null) return Enumerable.Empty<BO.OpenCallInList>();

                // Use the BL method to get open calls for volunteer
                var openCalls = s_bl.Call.GetOpenCallsForVolunteer(CurrentVolunteer.Id, callType, sortField);

                // Filter by distance
                return openCalls.Where(call => call.DistanceFromVolunteer <= CurrentVolunteer.MaxDistanceForTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving filtered open calls for volunteer {CurrentVolunteer?.FullName}: {ex.Message}",
                               "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Enumerable.Empty<BO.OpenCallInList>();
            }
        }

        /// <summary>
        /// Handles DataGrid selection changed event to display call description
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is OpenCallInList selectedCall)
            {
                try
                {
                    // Get full call details to access the description
                    var fullCall = s_bl.Call.GetCallDetails(selectedCall.Id);
                    CallDescription = fullCall?.Description ?? "אין תיאור זמין";
                }
                catch (Exception ex)
                {
                    CallDescription = $"שגיאה בקבלת תיאור הקריאה: {ex.Message}";
                }
            }
            else
            {
                CallDescription = "בחר קריאה כדי לראות את התיאור המפורט";
            }
        }

        /// <summary>
        /// Handles call selection and assignment to volunteer.
        /// </summary>
        private void OnSelectCall(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is OpenCallInList selectedCall)
            {
                try
                {
                    // Assign call to volunteer using business layer
                    s_bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, selectedCall.Id);

                    MessageBox.Show($"Call {selectedCall.Id} has been assigned to you.", "Success",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                    // List will refresh automatically via observers
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error assigning call: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles volunteer address update.
        /// </summary>
        private void OnUpdateAddress(object sender, RoutedEventArgs e)
        {
            try
            {
                var newAddress = ShowAddressInputDialog(CurrentVolunteer?.Address ?? "");
                if (!string.IsNullOrWhiteSpace(newAddress) && newAddress != CurrentVolunteer?.Address)
                {
                    CurrentVolunteer!.Address = newAddress;
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);

                    MessageBox.Show("Address updated successfully.", "Success",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    // Volunteer and calls will refresh automatically via observers
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating address: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Shows address input dialog and returns new address.
        /// </summary>
        /// <param name="currentAddress">Current volunteer address</param>
        /// <returns>New address or null if cancelled</returns>
        private string ShowAddressInputDialog(string currentAddress)
        {
            var inputDialog = new Window
            {
                Title = "Update Address",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = this
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var textBox = new TextBox { Text = currentAddress, Margin = new Thickness(0, 0, 0, 10) };
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            var okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(5) };
            var cancelButton = new Button { Content = "Cancel", Width = 75, Margin = new Thickness(5) };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(new TextBlock { Text = "Enter new address:", Margin = new Thickness(0, 0, 0, 5) });
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);
            inputDialog.Content = stackPanel;

            string newAddress = null;
            bool dialogResult = false;

            okButton.Click += (s, e) =>
            {
                newAddress = textBox.Text;
                dialogResult = true;
                inputDialog.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                dialogResult = false;
                inputDialog.Close();
            };

            inputDialog.ShowDialog();
            return dialogResult ? newAddress : null;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}