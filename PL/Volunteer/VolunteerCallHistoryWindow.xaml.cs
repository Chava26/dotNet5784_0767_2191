using System.Collections.Generic;
using System.ComponentModel; // נדרש עבור INotifyPropertyChanged
using System.Linq;
using System.Runtime.CompilerServices; // נדרש עבור [CallerMemberName]
using System.Windows;
using BlApi;
using BO;
//using static BO.Enums;

namespace PL
{
    public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();

        public VolunteerCallHistoryWindow()
        {
            InitializeComponent();
            VolunteerId = App.CurrentUserId;
            DataContext = this;
            QueryClosedCalls();
        }

        public int VolunteerId { get; set; }
        public IEnumerable<CallType> CallTypeCollection => Enum.GetValues(typeof(CallType)).Cast<CallType>();
        public IEnumerable<string> SortOptions => new[] { "Finish Time", "Type", "ID" };

        private CallType _selectedCallType = CallType.None;
        public CallType SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged(nameof(ClosedCalls));
                QueryClosedCalls(); // רענון הרשימה בעת שינוי
            }
        }
        private CallField _selectedSortOption = CallField.None;

        //private string _selectedSortOption = "Finish Time";
        public CallField SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged(nameof(ClosedCalls));
                QueryClosedCalls(); // רענון הרשימה בעת שינוי
            }
        }

        private IEnumerable<ClosedCallInList> _closedCalls = Enumerable.Empty<ClosedCallInList>(); // Initialize to an empty collection
        public IEnumerable<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                OnPropertyChanged(nameof(ClosedCalls));
            }
        }

        private void QueryClosedCalls()
        {
            var calls = s_bl.Call.GetClosedCallsByVolunteer(
                volunteerId: VolunteerId,
                callType: SelectedCallType == CallType.None ? null : SelectedCallType,
                sortField: SelectedSortOption ==  CallField.None ? null : SelectedSortOption
            );

            ClosedCalls = calls.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}