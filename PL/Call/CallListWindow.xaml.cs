using BlApi;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PL
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();

        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateCallList();
        }

        // אוספים ל-ComboBox-ים
        public IEnumerable<BO.CallType> CallTypeCollection =>
            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

        public IEnumerable<BO.CallStatus> CallStatusCollection =>
            Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>();

        public IEnumerable<string> SortFields => new[] { "Opening_time", "CallType", "CallStatus", "TotalAssignments" };
        public IEnumerable<string> GroupFields => new[] { "None", "CallType", "CallStatus" };

        // תכונות Binding
        private BO.CallType _selectedCallType = BO.CallType.None;
        public BO.CallType SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                if (_selectedCallType != value)
                {
                    _selectedCallType = value;
                    OnPropertyChanged(nameof(SelectedCallType));
                    UpdateCallList();
                }
            }
        }

        private BO.CallStatus? _selectedCallStatus = null;
        public BO.CallStatus? SelectedCallStatus
        {
            get => _selectedCallStatus;
            set
            {
                if (_selectedCallStatus != value)
                {
                    _selectedCallStatus = value;
                    OnPropertyChanged(nameof(SelectedCallStatus));
                    UpdateCallList();
                }
            }
        }

        private string _selectedSortField = "Opening_time";
        public string SelectedSortField
        {
            get => _selectedSortField;
            set
            {
                if (_selectedSortField != value)
                {
                    _selectedSortField = value;
                    OnPropertyChanged(nameof(SelectedSortField));
                    UpdateCallList();
                }
            }
        }

        private string _selectedGroupField = "None";
        public string SelectedGroupField
        {
            get => _selectedGroupField;
            set
            {
                if (_selectedGroupField != value)
                {
                    _selectedGroupField = value;
                    OnPropertyChanged(nameof(SelectedGroupField));
                    UpdateCallList();
                }
            }
        }

        // הרשימה בפועל
        private IEnumerable<CallInList> _callList;
        public IEnumerable<CallInList> CallList
        {
            get => _callList;
            set
            {
                _callList = value;
                OnPropertyChanged(nameof(CallList));
            }
        }

        private ListCollectionView _callListView;
        public ListCollectionView CallListView
        {
            get => _callListView;
            set
            {
                _callListView = value;
                OnPropertyChanged(nameof(CallListView));
            }
        }

        public OpenCallInList? SelectedCall { get; set; }
        // עדכון הרשימה לפי סינון/מיון/קיבוץ
        private void UpdateCallList()
        {
            // קביעת פרמטרי הסינון
            BO.CallField? filterField = null;
            object? filterValue = null;

            // הגדרת סינון - עדיפות ל-CallType על פני CallStatus
            if (SelectedCallType != BO.CallType.None)
            {
                filterField = BO.CallField.CallType;
                filterValue = SelectedCallType;
            }
            else if (SelectedCallStatus != null)
            {
                filterField = BO.CallField.Status;
                filterValue = SelectedCallStatus;
            }

            // הגדרת מיון
            BO.CallField? sortField = SelectedSortField switch
            {
                "CallType" => BO.CallField.CallType,
                "CallStatus" => BO.CallField.Status,
                "TotalAssignments" => BO.CallField.AssignmentsCount,
                _ => BO.CallField.OpenTime // ברירת מחדל
            };

            // קריאה לפונקציה המקורית עם הפרמטרים
            IEnumerable<CallInList> list = s_bl.Call.GetCalls(filterField, filterValue, sortField);

            // סינון נוסף במקרה שיש גם CallType וגם CallStatus מוגדרים
            // (הפונקציה המקורית מטפלת רק במסנן אחד)
            if (SelectedCallType != BO.CallType.None && SelectedCallStatus != null)
            {
                // אם סיננו לפי CallType, נוסיף סינון לפי Status
                if (filterField == BO.CallField.CallType)
                {
                    list = list.Where(c => c.Status == SelectedCallStatus);
                }
                // אם סיננו לפי Status, נוסיף סינון לפי CallType
                else if (filterField == BO.CallField.Status)
                {
                    list = list.Where(c => c.CallType == SelectedCallType);
                }
            }

            // המרה לרשימה
            //CallList = list.ToList();

            // קיבוץ
            var lcv = new ListCollectionView(list.ToList());
            lcv.GroupDescriptions.Clear();

            if (SelectedGroupField == "CallType")
                lcv.GroupDescriptions.Add(new PropertyGroupDescription("CallType"));
            else if (SelectedGroupField == "CallStatus")
                lcv.GroupDescriptions.Add(new PropertyGroupDescription("Status"));

            CallListView = lcv;
        }

        // עדכון הרשימה לפי סינון/מיון/קיבוץ
        //private void UpdateCallList()
        //{
        //    IEnumerable<CallInList> list = s_bl.Call.GetCalls();

        //    // סינון
        //    if (SelectedCallType != BO.CallType.None)
        //        list = list.Where(c => c.CallType == SelectedCallType);

        //    if (SelectedCallStatus != null)
        //        list = list.Where(c => c.Status == SelectedCallStatus);

        //    // מיון
        //    switch (SelectedSortField)
        //    {
        //        case "CallType":
        //            list = list.OrderBy(c => c.CallType);
        //            break;
        //        case "CallStatus":
        //            list = list.OrderBy(c => c.Status);
        //            break;
        //        case "TotalAssignments":
        //            list = list.OrderBy(c => c.AssignmentsCount);
        //            break;
        //        default:
        //            list = list.OrderBy(c => c.OpenTime);
        //            break;
        //    }

        //    CallList = list.ToList();

        //    // קיבוץ
        //    var lcv = new ListCollectionView(CallList.ToList());
        //    lcv.GroupDescriptions.Clear();
        //    if (SelectedGroupField == "CallType")
        //        lcv.GroupDescriptions.Add(new PropertyGroupDescription("CallType"));
        //    else if (SelectedGroupField == "CallStatus")
        //        lcv.GroupDescriptions.Add(new PropertyGroupDescription("CallStatus"));

        //    CallListView = lcv;
        //}

        // אירוע ל-ComboBox-ים (אם לא רוצים AutoUpdate דרך Setter)
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdateCallList();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CallInList call)
            {
                if (call.Status != BO.CallStatus.Open || call.AssignmentsCount > 0)
                {
                    MessageBox.Show("This call cannot be deleted. Only open calls with no assignments can be deleted.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete this call?", "Confirm Deletion",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(call.CallId);
                        UpdateCallList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting the call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            var addCallWindow = new AddCallWindow
            {
                Owner = this
            };

            if (addCallWindow.ShowDialog() == true)
            {
                UpdateCallList(); // רענון הרשימה אחרי הוספה
            }
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var window = new PL.Call.CallWindow(SelectedCall.Id);
                window.ShowDialog();
                UpdateCallList(); // רענון הרשימה לאחר סגירת החלון
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CallInList call)
            {
                // בדיקה: רק קריאה בסטטוס בטיפול עם מתנדב מוקצה
                if (call.Status != BO.CallStatus.InProgress || string.IsNullOrEmpty(call.LastVolunteerName))
                {
                    MessageBox.Show("ניתן לבטל הקצאה רק לקריאה בסטטוס 'בטיפול' עם מתנדב מוקצה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int? currentUserId = App.CurrentUserId;
                int? assignmentId = call.Id;
                if (currentUserId == null || assignmentId == null)
                {
                    MessageBox.Show("חסר מזהה משתמש או מזהה קריאה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show("האם לבטל את ההקצאה עבור קריאה זו? תישלח הודעה למתנדב.", "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.CancelAssignment(currentUserId.Value, assignmentId.Value);

                        UpdateCallList(); // רענון טבלה
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"שגיאה בביטול ההקצאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}