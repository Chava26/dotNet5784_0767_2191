
using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call
{
    public partial class SelectCallForTreatmentWindow : Window, INotifyPropertyChanged
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

        private ObservableCollection<OpenCallInList>? _openCalls;
        private string _callDescription = "בחר קריאה כדי לראות את התיאור המפורט";

        public ObservableCollection<OpenCallInList>? OpenCalls
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
            Close();
        }


        /// <summary>
        /// Refreshes volunteer data when it changes in the business layer.
        /// </summary>
        private void RefreshVolunteer()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                    RefreshOpenCalls(); 
                }
            );
        }

        /// <summary>
        /// Refreshes open calls list when data changes in the business layer.
        /// </summary>
        private void RefreshOpenCalls()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {

                    try
                    {
                    // Unregister observers for old calls

                    var filteredCalls = GetFilteredOpenCalls();
                    OpenCalls = new ObservableCollection<OpenCallInList>(filteredCalls);

                    // Register observers for new calls
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing open calls: {ex.Message}", "Error",
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
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}