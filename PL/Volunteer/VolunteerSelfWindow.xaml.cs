////using BlApi;
////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Data;
////using System.Windows.Documents;
////using System.Windows.Input;
////using System.Windows.Media;
////using System.Windows.Media.Imaging;
////using System.Windows.Shapes;

////namespace PL.Volunteer
////{
////    /// <summary>
////    /// Interaction logic for VolunteerSelfWindow.xaml
////    /// </summary>
////    public partial class VolunteerSelfWindow : Window
////    {
////        private static readonly IBl s_bl = BlApi.Factory.Get();

////        public BO.Volunteer Volunteer { get; set; }
////        public static IEnumerable<BO.DistanceType> DistanceTypes => Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

////        public bool HasCallInProgress => Volunteer?.callInProgress != null;
////        public bool CanSelectCall => Volunteer?.callInProgress == null && Volunteer?.IsActive == true;
////        public bool CanSetInactive => Volunteer?.callInProgress == null;

////        public VolunteerSelfWindow(int volunteerId)
////        {
////            InitializeComponent();
////            Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
////            DataContext = this;
////        }
////        private void btnUpdate_Click(object sender, RoutedEventArgs e)
////        {
////            try
////            {
////                //PasswordBox passwordBox = this.FindName("PasswordBox") as PasswordBox;


////                //if (passwordBox != null && !string.IsNullOrWhiteSpace(passwordBox.Password))
////                //    Volunteer.Password = passwordBox.Password;
////                //else
////                //    Volunteer.Password = null;"PasswordBox.Password" is not null && PasswordBox.Password != string.Empty ? PasswordBox.Password : null;
////                if (Password is not null && Password != string.Empty )
////                    Volunteer.Password = Password;

////                s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
////                MessageBox.Show("הפרטים עודכנו בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
////                //Close();
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("שגיאה בעדכון הפרטים: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
////            }

////        }

////        /// <summary>
////        /// The password input by the user (bound to password box).
////        /// </summary>
////        public string Password
////        {
////            get => (string)GetValue(PasswordProperty);
////            set => SetValue(PasswordProperty, value);
////        }

////        public static readonly DependencyProperty PasswordProperty =
////            DependencyProperty.Register(
////                nameof(Password),
////                typeof(string),
////                typeof(VolunteerWindow),
////                new PropertyMetadata(string.Empty));

////        /// <summary>
////        /// Updates the Password property when the password box is changed.
////        /// </summary>
////        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
////        {
////            var passwordBox = sender as PasswordBox;
////            if (passwordBox != null)
////                Password = passwordBox.Password;
////        }

////        private void btnFinishCall_Click(object sender, RoutedEventArgs e)
////        {
////            if (Volunteer?.callInProgress == null) return;
////            try
////            {
////                s_bl.Call.CompleteCall(Volunteer.Id, Volunteer.callInProgress.Id);
////                MessageBox.Show("הטיפול בקריאה הסתיים.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
////                Volunteer = s_bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
////                DataContext = null;
////                DataContext = this;
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("שגיאה בסיום טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
////            }
////        }

////        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
////        {
////            if (Volunteer?.callInProgress == null) return;
////            try
////            {
////                s_bl.Call.CancelAssignment(Volunteer.Id, Volunteer.callInProgress.Id);
////                MessageBox.Show("הטיפול בקריאה בוטל.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
//                Volunteer = s_bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
//                DataContext = null;
//                DataContext = this;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        //private void btnHistory_Click(object sender, RoutedEventArgs e)
//        //{
//        //    // פתח מסך היסטוריית קריאות (יש לממש מסך זה)
//        //    var historyWindow = new VolunteerHistoryWindow(Volunteer.Id);
//        //    historyWindow.ShowDialog();
//        //}
//    }
using BlApi;
using PL.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerSelfWindow.xaml
    /// </summary>
    public partial class VolunteerSelfWindow : Window
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// The volunteer object as a dependency property for proper binding
        /// </summary>
        public BO.Volunteer? Volunteer
        {
            get => (BO.Volunteer?)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register(nameof(Volunteer), typeof(BO.Volunteer), typeof(VolunteerSelfWindow), new PropertyMetadata(null));

        public static IEnumerable<BO.DistanceType> DistanceTypes => Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        public bool HasCallInProgress => Volunteer?.callInProgress != null;
        public bool CanSelectCall => Volunteer?.callInProgress == null && Volunteer?.IsActive == true;
        public bool CanSetInactive => Volunteer?.callInProgress == null;

        /// <summary>
        /// The password input by the user (bound to password box).
        /// </summary>
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(VolunteerSelfWindow),
                new PropertyMetadata(string.Empty));

        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
            DataContext = this;

            // Subscribe to observer pattern for automatic updates
            if (Volunteer != null)
            {
                s_bl.Volunteer.AddObserver(Volunteer.Id, RefreshVolunteer);
            }

            // Subscribe to window closed event to clean up observer
            Closed += Window_Closed;
        }

        /// <summary>
        /// Unsubscribes from data updates on window close.
        /// </summary>
        private void Window_Closed(object? sender, EventArgs e)
        {
            if (Volunteer != null)
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteer);
        }

        /// <summary>
        /// Reloads the volunteer details from the business logic layer.
        /// </summary>
        private void RefreshVolunteer()
        {
            if (Volunteer == null) return;

            int id = Volunteer.Id;
            Volunteer = null;
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            DataContext = null;
            DataContext = this;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer == null) return;

                Volunteer.Password = Password;

                s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear password after successful update
                Password = string.Empty;
                if (PasswordBox != null)
                    PasswordBox.Clear();

                // The RefreshVolunteer method will be called automatically by the observer
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון הפרטים: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the Password property when the password box is changed.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
                Password = passwordBox.Password;
        }

        private void btnFinishCall_Click(object sender, RoutedEventArgs e)
        {
            if (Volunteer?.callInProgress == null) return;
            try
            {
                s_bl.Call.CompleteCall(Volunteer.Id, Volunteer.callInProgress.Id);
                MessageBox.Show("הטיפול בקריאה הסתיים.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVolunteer();
                // The RefreshVolunteer method will be called automatically by the observer
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בסיום טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (Volunteer?.callInProgress == null) return;
            try
            {
                s_bl.Call.CancelAssignment(Volunteer.Id, Volunteer.callInProgress.Id);
                MessageBox.Show("הטיפול בקריאה בוטל.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVolunteer();
                // The RefreshVolunteer method will be called automatically by the observer
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new VolunteerCallHistoryWindow();
            historyWindow.ShowDialog();
        }
        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new SelectCallForTreatmentWindow();
            historyWindow.ShowDialog();
        }
    }
}

