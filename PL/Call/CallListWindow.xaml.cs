//using BlApi;
//using BO;
//using DO;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Input;

//namespace PL
//{
//    public partial class CallListWindow : Window, INotifyPropertyChanged
//    {
//        private static readonly IBl s_bl = Factory.Get();

//        public CallListWindow()
//        {
//            InitializeComponent();
//            DataContext = this;
//        }

//        // אוספים ל-ComboBox-ים
//        public IEnumerable<BO.CallType> CallTypeCollection =>
//            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

//        public IEnumerable<BO.CallStatus> CallStatusCollection =>
//            Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>();

//        public IEnumerable<string> SortFields => new[] { "Opening_time", "CallType", "CallStatus", "TotalAssignments" };
//        public IEnumerable<string> GroupFields => new[] { "None", "CallType", "CallStatus" };

//        // משתני Binding
//        private BO.CallType _selectedCallType = BO.CallType.None;
//        public BO.CallType SelectedCallType
//        {
//            get => _selectedCallType;
//            set
//            {
//                if (_selectedCallType != value)
//                {
//                    _selectedCallType = value;
//                    OnPropertyChanged(nameof(SelectedCallType));
//                    UpdateCallList();
//                }
//            }
//        }

//        private BO.CallStatus? _selectedCallStatus = null;
//        public BO.CallStatus? SelectedCallStatus
//        {
//            get => _selectedCallStatus;
//            set
//            {
//                if (_selectedCallStatus != value)
//                {
//                    _selectedCallStatus = value;
//                    OnPropertyChanged(nameof(SelectedCallStatus));
//                    UpdateCallList();
//                }
//            }
//        }

//        private string _selectedSortField = "Opening_time";
//        public string SelectedSortField
//        {
//            get => _selectedSortField;
//            set
//            {
//                if (_selectedSortField != value)
//                {
//                    _selectedSortField = value;
//                    OnPropertyChanged(nameof(SelectedSortField));
//                    UpdateCallList();
//                }
//            }
//        }

//        private string _selectedGroupField = "None";
//        public string SelectedGroupField
//        {
//            get => _selectedGroupField;
//            set
//            {
//                if (_selectedGroupField != value)
//                {
//                    _selectedGroupField = value;
//                    OnPropertyChanged(nameof(SelectedGroupField));
//                    UpdateCallList();
//                }
//            }
//        }

//        // הרשימה בפועל
//        private ListCollectionView _callListView;
//        public ListCollectionView CallListView
//        {
//            get => _callListView;
//            set
//            {
//                _callListView = value;
//                OnPropertyChanged(nameof(CallListView));
//            }
//        }

//        // הקריאה הנבחרת
//        private CallInList? _selectedCall;
//        public CallInList? SelectedCall
//        {
//            get => _selectedCall;
//            set
//            {
//                _selectedCall = value;
//                OnPropertyChanged(nameof(SelectedCall));
//            }
//        }

//        /// <summary>
//        /// Registers to business layer observers and loads initial call list.
//        /// </summary>
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            // רישום למשקיפים ברמת הלוגיקה
//            s_bl.Call.AddObserver(RefreshCallList);

//            // טעינה ראשונית של הרשימה
//            UpdateCallList();
//        }

//        /// <summary>
//        /// Unregisters from business layer observers on window close.
//        /// </summary>
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            // ביטול רישום למשקיפים
//            s_bl.Call.RemoveObserver(RefreshCallList);
//        }

//        /// <summary>
//        /// Refreshes call list automatically when business layer data changes.
//        /// </summary>
//        private void RefreshCallList()
//        {
//            // קריאה לעדכון בthread הנכון של ה-UI
//            Dispatcher.Invoke(() =>
//            {
//                UpdateCallList();
//            });
//        }

//        /// <summary>
//        /// Updates call list with current filter, sort, and grouping settings.
//        /// </summary>
//        private void UpdateCallList()
//        {
//            try
//            {
//                // קביעת פרמטרי הסינון
//                BO.CallField? filterField = null;
//                object? filterValue = null;

//                // הגדרת סינון - עדיפות ל-CallType על פני CallStatus
//                if (SelectedCallType != BO.CallType.None)
//                {
//                    filterField = BO.CallField.CallType;
//                    filterValue = SelectedCallType;
//                }
//                else if (SelectedCallStatus != null)
//                {
//                    filterField = BO.CallField.Status;
//                    filterValue = SelectedCallStatus;
//                }

//                // הגדרת מיון
//                BO.CallField? sortField = SelectedSortField switch
//                {
//                    "CallType" => BO.CallField.CallType,
//                    "CallStatus" => BO.CallField.Status,
//                    "TotalAssignments" => BO.CallField.AssignmentsCount,
//                    _ => BO.CallField.OpenTime // ברירת מחדל
//                };

//                // קריאה לפונקציה הנכונית עם הפרמטרים
//                IEnumerable<CallInList> list = s_bl.Call.GetCalls(filterField, filterValue, sortField);

//                // סינון נוסף במקרה שיש גם CallType וגם CallStatus מוגדרים
//                if (SelectedCallType != BO.CallType.None && SelectedCallStatus != null)
//                {
//                    if (filterField == BO.CallField.CallType)
//                    {
//                        list = list.Where(c => c.Status == SelectedCallStatus);
//                    }
//                    else if (filterField == BO.CallField.Status)
//                    {
//                        list = list.Where(c => c.CallType == SelectedCallType);
//                    }
//                }

//                // קיבוץ
//                var lcv = new ListCollectionView(list.ToList());
//                lcv.GroupDescriptions.Clear();

//                if (SelectedGroupField == "CallType")
//                    lcv.GroupDescriptions.Add(new PropertyGroupDescription("CallType"));
//                else if (SelectedGroupField == "CallStatus")
//                    lcv.GroupDescriptions.Add(new PropertyGroupDescription("Status"));

//                CallListView = lcv;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error updating call list: {ex.Message}", "Error",
//                               MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// ComboBox selection changed event handler (legacy compatibility).
//        /// </summary>
//        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
//        {
//            // לא צריך לעשות כלום - הSetter כבר קורא ל-UpdateCallList
//        }

//        /// <summary>
//        /// Deletes selected call after validation and confirmation.
//        /// </summary>
//        private void DeleteCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button btn && btn.Tag is CallInList call)
//            {
//                if ((call.Status != BO.CallStatus.Open && call.Status != BO.CallStatus.OpenRisk) || call.AssignmentsCount > 0)
//                {
//                    MessageBox.Show("לא ניתן למחוק קריאה זו. ניתן למחוק רק קריאות פתוחות ללא הקצאות.",
//                                    "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }

//                if (MessageBox.Show("האם אתה בטוח שברצונך למחוק קריאה זו?", "אישור מחיקה",
//                                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
//                {
//                    try
//                    {
//                        s_bl.Call.DeleteCall(call.CallId);
//                        // הרשימה תתעדכן אוטומטית דרך המשקיף
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"שגיאה במחיקת הקריאה: {ex.Message}", "שגיאה",
//                                       MessageBoxButton.OK, MessageBoxImage.Error);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Opens add call dialog window with proper auto-close handling.
//        /// </summary>
//        private void AddCall_Click(object sender, RoutedEventArgs e)
//        {
//            var addCallWindow = new AddCallWindow
//            {
//                Owner = this
//            };

//            // ShowDialog מחזיר true אם הקריאה נוספה בהצלחה
//            if (addCallWindow.ShowDialog() == true)
//            {
//                // הרשימה תתעדכן אוטומטית דרך המשקיף
//                // אין צורך בקריאה מפורשת ל-UpdateCallList
//            }
//        }

//        /// <summary>
//        /// Opens call details window on double-click.
//        /// </summary>
//        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            if (SelectedCall != null)
//            {
//                var window = new PL.Call.CallWindow(SelectedCall.CallId)
//                {
//                    Owner = this
//                };
//                window.ShowDialog();    // הרשימה תתעדכן אוטומטית דרך המשקיף
//            }
//        }

//        /// <summary>
//        /// Cancels volunteer assignment for selected call.
//        /// </summary>
//        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button btn && btn.Tag is CallInList call)
//            {
//                // בדיקה: רק קריאה בסטטוס בטיפול עם מתנדב מוקצה
//                if ((call.Status != BO.CallStatus.InProgress && call.Status != BO.CallStatus.InProgressRisk) || string.IsNullOrEmpty(call.LastVolunteerName))
//                {
//                    MessageBox.Show("ניתן לבטל הקצאה רק לקריאה בסטטוס 'בטיפול' עם מתנדב מוקצה.",
//                                   "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }

//                int currentUserId = App.CurrentUserId;
//                int? assignmentId = call.Id;

//                if (assignmentId == null)
//                {
//                    MessageBox.Show("חסר מזהה משימה או מזהה קריאה.", "שגיאה",
//                                   MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }

//                if (MessageBox.Show("האם לבטל את ההקצאה עבור קריאה זו? תישלח הודעה למתנדב.",
//                                   "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
//                {
//                    try
//                    {
//                        s_bl.Call.CancelAssignment(currentUserId, assignmentId.Value);
//                        // הרשימה תתעדכן אוטומטית דרך המשקיף
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"שגיאה בביטול ההקצאה: {ex.Message}", "שגיאה",
//                                       MessageBoxButton.OK, MessageBoxImage.Error);
//                    }
//                }
//            }
//        }

//        // INotifyPropertyChanged implementation
//        public event PropertyChangedEventHandler? PropertyChanged;
//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
//       
using BlApi;
using BO;
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

        // הקריאה הנבחרת
        private CallInList? _selectedCall;
        public CallInList? SelectedCall
        {
            get => _selectedCall;
            set
            {
                _selectedCall = value;
                OnPropertyChanged(nameof(SelectedCall));
            }
        }

        /// <summary>
        /// Registers to business layer observers and loads initial call list.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // רישום למשקיפים ברמת הלוגיקה
            s_bl.Call.AddObserver(RefreshCallList);

            // טעינה ראשונית של הרשימה
            UpdateCallList();
        }

        /// <summary>
        /// Unregisters from business layer observers on window close.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // ביטול רישום למשקיפים
            s_bl.Call.RemoveObserver(RefreshCallList);
        }

        /// <summary>
        /// Refreshes call list automatically when business layer data changes.
        /// </summary>
        private void RefreshCallList()
        {
            // קריאה לעדכון בthread הנכון של ה-UI
            Dispatcher.Invoke(() =>
            {
                UpdateCallList();
            });
        }

        /// <summary>
        /// Updates call list with current filter, sort, and grouping settings.
        /// </summary>
        private void UpdateCallList()
        {
            try
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
                if (SelectedCallType != BO.CallType.None && SelectedCallStatus != null)
                {
                    if (filterField == BO.CallField.CallType)
                    {
                        list = list.Where(c => c.Status == SelectedCallStatus);
                    }
                    else if (filterField == BO.CallField.Status)
                    {
                        list = list.Where(c => c.CallType == SelectedCallType);
                    }
                }

                // קיבוץ
                var lcv = new ListCollectionView(list.ToList());
                lcv.GroupDescriptions.Clear();

                if (SelectedGroupField == "CallType")
                    lcv.GroupDescriptions.Add(new PropertyGroupDescription("CallType"));
                else if (SelectedGroupField == "CallStatus")
                    lcv.GroupDescriptions.Add(new PropertyGroupDescription("Status"));

                CallListView = lcv;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating call list: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// ComboBox selection changed event handler (legacy compatibility).
        /// </summary>
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            // לא צריך לעשות כלום - הSetter כבר קורא ל-UpdateCallList
        }

        /// <summary>
        /// Deletes selected call after validation and confirmation.
        /// </summary>
        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CallInList call)
            {
                if ((call.Status != BO.CallStatus.Open && call.Status != BO.CallStatus.OpenRisk) || call.AssignmentsCount > 0)
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting the call: {ex.Message}", "Error",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Opens add call dialog window.
        /// </summary>
        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            var addCallWindow = new AddCallWindow();
            addCallWindow.Show();
            //{
            //    Owner = this
            //};

            //if (addCallWindow.ShowDialog() == true)
            //{
            //    // הרשימה תתעדכן אוטומטית דרך המשקיף
            //}
        }

        /// <summary>
        /// Opens call details window on double-click.
        /// </summary>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var window = new PL.Call.CallWindow(SelectedCall.CallId);
                window.Show();    // הרשימה תתעדכן אוטומטית דרך המשקיף
            }
        }

        /// <summary>
        /// Cancels volunteer assignment for selected call.
        /// </summary>
        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CallInList call)
            {
                // בדיקה: רק קריאה בסטטוס בטיפול עם מתנדב מוקצה
                if ((call.Status != BO.CallStatus.InProgress && call.Status != BO.CallStatus.InProgressRisk) || string.IsNullOrEmpty(call.LastVolunteerName))
                {
                    MessageBox.Show("ניתן לבטל הקצאה רק לקריאה בסטטוס 'בטיפול' עם מתנדב מוקצה.",
                                   "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int currentUserId = App.CurrentUserId;
                int? assignmentId = call.Id;

                if (assignmentId == null)
                {
                    MessageBox.Show("חסר מזהה משתמש או מזהה קריאה.", "שגיאה",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show("האם לבטל את ההקצאה עבור קריאה זו? תישלח הודעה למתנדב.",
                                   "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.CancelAssignment(currentUserId, assignmentId.Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"שגיאה בביטול ההקצאה: {ex.Message}", "שגיאה",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}