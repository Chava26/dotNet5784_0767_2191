using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        /// <summary>
        /// Access to the business logic layer.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7



        /// <summary>
        /// The currently selected volunteer from the list view.
        /// </summary>
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        /// <summary>
        /// Initializes the window, loads the volunteer list, and registers the update observer.
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();
            GetVolunteerList();
        }

        /// <summary>
        /// Loads and filters the volunteer list based on current sort and filter selections.
        /// </summary>
        private void GetVolunteerList()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    VolunteerList = s_bl.Volunteer.GetVolunteersList(
                isActive: null,
                sortBy: SelectedSortField == BO.VolunteerSortField.None ? null : SelectedSortField,
                filterField: SelectedCallType == BO.CallType.None ? null : SelectedCallType);
                });
                }

        /// <summary>
        /// The list of volunteers displayed in the UI.
        /// </summary>
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }

        /// <summary>
        /// Dependency property for VolunteerList.
        /// </summary>
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        private BO.VolunteerSortField _selectedSortField = BO.VolunteerSortField.None;

        /// <summary>
        /// The field by which the volunteer list is currently sorted.
        /// </summary>
        public BO.VolunteerSortField SelectedSortField
        {
            get => _selectedSortField;
            set
            {
                if (_selectedSortField != value)
                {
                    _selectedSortField = value;
                    GetVolunteerList();
                }
            }
        }

        private BO.CallType? _selectedCallType = null;

        /// <summary>
        /// The call type used for filtering the volunteer list.
        /// </summary>
        public BO.CallType? SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                if (_selectedCallType != value)
                {
                    _selectedCallType = value;
                    GetVolunteerList();
                }
            }
        }

        /// <summary>
        /// Mapping between combo box display strings and sort field enums.
        /// </summary>
        private static readonly Dictionary<string, BO.VolunteerSortField> _sortFieldMap =
            new Dictionary<string, BO.VolunteerSortField>
            {
                { "Name", BO.VolunteerSortField.Name },
                { "Total Responses Handled", BO.VolunteerSortField.TotalHandledCalls },
                { "Total Responses Cancelled", BO.VolunteerSortField.TotalCanceledCalls },
                { "Total Expired Responses", BO.VolunteerSortField.TotalExpiredCalls },
                { "Sum of Cancellation", BO.VolunteerSortField.SumOfCancellation },
                { "Sum of Expired Calls", BO.VolunteerSortField.SumOfExpiredCalls },
            };

        /// <summary>
        /// Handles the selection change in the sort field combo box.
        /// </summary>
        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            var content = selectedItem.Content.ToString();
            SelectedSortField = content != null && _sortFieldMap.TryGetValue(content, out var field) ? field : BO.VolunteerSortField.None;
        }

        /// <summary>
        /// Converts a string to the corresponding sort field enum.
        /// </summary>
        public static BO.VolunteerSortField FromString(string fieldName)
        {
            return _sortFieldMap.TryGetValue(fieldName, out var value)
                ? value
                : BO.VolunteerSortField.None;
        }

        /// <summary>
        /// Registers the observer when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e) { s_bl?.Volunteer.AddObserver(GetVolunteerList);
        }

        /// <summary>
        /// Unregisters the observer when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e) { s_bl?.Volunteer.RemoveObserver(GetVolunteerList);
        }

        /// <summary>
        /// Opens a window for adding a new volunteer.
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e) => new VolunteerWindow().Show();

        /// <summary>
        /// Opens a window to edit the selected volunteer on double click.
        /// </summary>
        private void lsvVolunteerList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(true, SelectedVolunteer.Id).Show();
        }

        /// <summary>
        /// Deletes the selected volunteer after confirmation.
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteer = ExtractVolunteerFromSender(sender);
            if (volunteer == null) return;

            if (!AskUserDeleteConfirmation(volunteer)) return;

            if (DeleteVolunteerWithFeedback(volunteer.Id))
            {
                GetVolunteerList();
            }
        }

        /// <summary>
        /// Extracts the VolunteerInList object from the sender button.
        /// </summary>
        private BO.VolunteerInList? ExtractVolunteerFromSender(object sender)
        {
            return (sender as Button)?.DataContext as BO.VolunteerInList;
        }

        /// <summary>
        /// Asks the user to confirm volunteer deletion.
        /// </summary>
        private bool AskUserDeleteConfirmation(BO.VolunteerInList volunteer)
        {
            var msg = $"Delete volunteer '{volunteer.FullName}' (ID: {volunteer.Id})?";
            return MessageBox.Show(msg, "Delete Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Deletes the volunteer and shows appropriate feedback.
        /// </summary>
        private bool DeleteVolunteerWithFeedback(int volunteerId)
        {
            try
            {
                s_bl.Volunteer.DeleteVolunteer(volunteerId);
                MessageBox.Show("Volunteer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (BO.BlDeletionException ex)
            {
                MessageBox.Show($"Cannot delete volunteer:\n{ex.Message}", "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
