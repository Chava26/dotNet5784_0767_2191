////using System.Collections.Generic;
////using System.ComponentModel; // נדרש עבור INotifyPropertyChanged
////using System.Linq;
////using System.Runtime.CompilerServices; // נדרש עבור [CallerMemberName]
////using System.Windows;
////using BlApi;
////using BO;
//////using static BO.Enums;

////namespace PL
////{
////    public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
////    {
////        private static readonly IBl s_bl = Factory.Get();

////        public VolunteerCallHistoryWindow()
////        {
////            InitializeComponent();
////            VolunteerId = App.CurrentUserId;
////            DataContext = this;
////            QueryClosedCalls();
////        }

////        public int VolunteerId { get; set; }
////        public IEnumerable<CallType> CallTypeCollection => Enum.GetValues(typeof(CallType)).Cast<CallType>();
////        public IEnumerable<string> SortOptions => new[] { "Finish Time", "Type", "ID" };

////        private CallType _selectedCallType = CallType.None;
////        public CallType SelectedCallType
////        {
////            get => _selectedCallType;
////            set
////            {
////                _selectedCallType = value;
////                OnPropertyChanged(nameof(ClosedCalls));
////                QueryClosedCalls(); // רענון הרשימה בעת שינוי
////            }
////        }
////        private CallField _selectedSortOption = CallField.None;

////        //private string _selectedSortOption = "Finish Time";
////        public CallField SelectedSortOption
////        {
////            get => _selectedSortOption;
////            set
////            {
////                _selectedSortOption = value;
////                OnPropertyChanged(nameof(ClosedCalls));
////                QueryClosedCalls(); // רענון הרשימה בעת שינוי
////            }
////        }

////        private IEnumerable<ClosedCallInList> _closedCalls = Enumerable.Empty<ClosedCallInList>(); // Initialize to an empty collection
////        public IEnumerable<ClosedCallInList> ClosedCalls
////        {
////            get => _closedCalls;
////            set
////            {
////                _closedCalls = value;
////                OnPropertyChanged(nameof(ClosedCalls));
////            }
////        }

////        private void QueryClosedCalls()
////        {
////            var calls = s_bl.Call.GetClosedCallsByVolunteer(
////                volunteerId: VolunteerId,
////                callType: SelectedCallType == CallType.None ? null : SelectedCallType,
////                sortField: SelectedSortOption ==  CallField.None ? null : SelectedSortOption
////            );

////            ClosedCalls = calls.ToList();
////        }

////        public event PropertyChangedEventHandler? PropertyChanged;

////        //private void OnPropertyChanged(string propertyName)
////        //{
////        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
////        //}

////        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
////        {
////            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
////        }
////    }
////}
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using BlApi;
//using BO;

//namespace PL
//{
//    public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
//    {
//        private static readonly IBl s_bl = Factory.Get();

//        public VolunteerCallHistoryWindow()
//        {
//            InitializeComponent();
//            VolunteerId = App.CurrentUserId;
//            DataContext = this;
//            QueryClosedCalls();
//        }

//        public int VolunteerId { get; set; }

//        public IEnumerable<CallType> CallTypeCollection =>
//            new[] { CallType.None }.Concat(System.Enum.GetValues(typeof(CallType)).Cast<CallType>().Where(x => x != CallType.None));

//        public IEnumerable<CallField> SortOptions =>
//            new[] { CallField.None, CallField.TreatmentCompletionTime, CallField.CallType, CallField.CallId };

//        private CallType _selectedCallType = CallType.None;
//        public CallType SelectedCallType
//        {
//            get => _selectedCallType;
//            set
//            {
//                _selectedCallType = value;
//                OnPropertyChanged();
//                QueryClosedCalls();
//            }
//        }

//        private CallField _selectedSortOption = CallField.None;
//        public CallField SelectedSortOption
//        {
//            get => _selectedSortOption;
//            set
//            {
//                _selectedSortOption = value;
//                OnPropertyChanged();
//                QueryClosedCalls();
//            }
//        }

//        private IEnumerable<ClosedCallInList> _closedCalls = Enumerable.Empty<ClosedCallInList>();
//        public IEnumerable<ClosedCallInList> ClosedCalls
//        {
//            get => _closedCalls;
//            set
//            {
//                _closedCalls = value;
//                OnPropertyChanged();
//            }
//        }

//        private void QueryClosedCalls()
//        {
//            var calls = s_bl.Call.GetClosedCallsByVolunteer(
//                volunteerId: VolunteerId,
//                callType: SelectedCallType == CallType.None ? null : SelectedCallType,
//                sortField: SelectedSortOption == CallField.None ? null : SelectedSortOption
//            );
//            ClosedCalls = calls.ToList();
//        }

//        private void OnClose(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            QueryClosedCalls();
//        }

//        public event PropertyChangedEventHandler? PropertyChanged;

//        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}
using BlApi;
using BO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace PL
{
    public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        public VolunteerCallHistoryWindow()
        {
            InitializeComponent();
            VolunteerId = App.CurrentUserId;
            DataContext = this;

            // Add observer for call updates
            Loaded += Window_Loaded;
            Closed += Window_Closed;

            QueryClosedCalls();
        }

        public int VolunteerId { get; set; }

        public IEnumerable<CallType> CallTypeCollection =>
            new[] { CallType.None }.Concat(System.Enum.GetValues(typeof(CallType)).Cast<CallType>().Where(x => x != CallType.None));

        public IEnumerable<CallField> SortOptions =>
            new[] { CallField.None, CallField.TreatmentCompletionTime, CallField.CallType, CallField.CallId };

        private CallType _selectedCallType = CallType.None;
        public CallType SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged();
                QueryClosedCalls();
            }
        }

        private CallField _selectedSortOption = CallField.None;
        public CallField SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                QueryClosedCalls();
            }
        }

        private IEnumerable<ClosedCallInList> _closedCalls = Enumerable.Empty<ClosedCallInList>();
        public IEnumerable<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Subscribes to call updates when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to general call updates for this volunteer
            s_bl.Volunteer.AddObserver(VolunteerId, RefreshCallHistory);
            QueryClosedCalls();
        }

        /// <summary>
        /// Unsubscribes from call updates when the window is closed.
        /// </summary>
        private void Window_Closed(object? sender, System.EventArgs e)
        {
            // Unsubscribe from call updates
            s_bl.Call.RemoveObserver(VolunteerId, RefreshCallHistory);
        }

        /// <summary>
        /// Refreshes the call history from the business logic layer.
        /// </summary>
        private void RefreshCallHistory()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    // Refresh the call history when notified of changes
                    QueryClosedCalls();

                });
        }

        private void QueryClosedCalls()
        {
            var calls = s_bl.Call.GetClosedCallsByVolunteer(
                volunteerId: VolunteerId,
                callType: SelectedCallType == CallType.None ? null : SelectedCallType,
                sortField: SelectedSortOption == CallField.None ? null : SelectedSortOption
            );
            ClosedCalls = calls.ToList();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}