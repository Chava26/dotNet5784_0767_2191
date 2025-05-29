using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public VolunteerListWindow()
        {
            InitializeComponent();
            SortFields = Enum.GetValues(typeof(BO.VolunteerSortField)).Cast<BO.VolunteerSortField>().ToList();
            GetVolunteerList();
            s_bl?.Volunteer.AddObserver(GetVolunteerList);
        }

        private void GetVolunteerList()
        {
            VolunteerList = s_bl.Volunteer.GetVolunteersList(
                isActive: null,
                sortBy: SelectedSortField == BO.VolunteerSortField.None ? null : SelectedSortField,
               filterField: SelectedCallType == BO.CallType.None ? null : SelectedCallType);

        }


        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public List<BO.VolunteerSortField> SortFields { get; set; }

        private BO.VolunteerSortField _selectedSortField = BO.VolunteerSortField.None;
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
       
        public IEnumerable<BO.CallType> CallTypeList { get; } = Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>().ToList();

        private BO.CallType? _selectedCallType = null;
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


        private static readonly Dictionary<string, BO.VolunteerSortField> _sortFieldMap =
          new Dictionary<string, BO.VolunteerSortField>
          {
        { "Name", BO.VolunteerSortField.Name },
        { "Total Responses Handled", BO.VolunteerSortField.TotalHandledCalls },
        { "Total Responses Cancelled", BO.VolunteerSortField.TotalCanceledCalls },
        { "Total Expired Responses", BO.VolunteerSortField.TotalExpiredCalls },
        //{ "Sum of Calls", BO.VolunteerSortField.SumOfCalls },
        { "Sum of Cancellation", BO.VolunteerSortField.SumOfCancellation },
        { "Sum of Expired Calls", BO.VolunteerSortField.SumOfExpiredCalls },
          };

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            var content = selectedItem.Content.ToString();
            SelectedSortField = content != null && _sortFieldMap.TryGetValue(content, out var field) ? field: BO.VolunteerSortField.None;
        }

        public static BO.VolunteerSortField FromString(string fieldName)
        {
            return _sortFieldMap.TryGetValue(fieldName, out var value)
                ? value
                : BO.VolunteerSortField.None;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl?.Volunteer.AddObserver(GetVolunteerList);

        private void Window_Closed(object sender, EventArgs e) => s_bl?.Volunteer.RemoveObserver(GetVolunteerList);

        private void btnAdd_Click(object sender, RoutedEventArgs e) => new VolunteerWindow().Show();


        private void lsvVolunteerList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(true,SelectedVolunteer.Id).Show();

        }
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

        private BO.VolunteerInList? ExtractVolunteerFromSender(object sender)
        {
            return (sender as Button)?.DataContext as BO.VolunteerInList;
        }

        private bool AskUserDeleteConfirmation(BO.VolunteerInList volunteer)
        {
            var msg = $"Delete volunteer '{volunteer.FullName}' (ID: {volunteer.Id})?";
            return MessageBox.Show(msg, "Delete Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

       
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

        
    }
}

