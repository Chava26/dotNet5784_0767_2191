
//using BlApi;
//using PL.Call;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

//namespace PL.Volunteer
//{
//    /// <summary>
//    /// Interaction logic for VolunteerSelfWindow.xaml
//    /// </summary>
//    public partial class VolunteerSelfWindow : Window
//    {
//        private static readonly IBl s_bl = BlApi.Factory.Get();

//        /// <summary>
//        /// The volunteer object as a dependency property for proper binding
//        /// </summary>
//        public BO.Volunteer? Volunteer
//        {
//            get => (BO.Volunteer?)GetValue(VolunteerProperty);
//            set => SetValue(VolunteerProperty, value);
//        }

//        public static readonly DependencyProperty VolunteerProperty =
//            DependencyProperty.Register(nameof(Volunteer), typeof(BO.Volunteer), typeof(VolunteerSelfWindow), new PropertyMetadata(null));

//        public static IEnumerable<BO.DistanceType> DistanceTypes => Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

//        public bool HasCallInProgress => Volunteer?.callInProgress != null;
//        public bool CanSelectCall => Volunteer?.callInProgress == null && Volunteer?.IsActive == true;
//        public bool CanSetInactive => Volunteer?.callInProgress == null;

//        /// <summary>
//        /// The password input by the user (bound to password box).
//        /// </summary>
//        public string Password
//        {
//            get => (string)GetValue(PasswordProperty);
//            set => SetValue(PasswordProperty, value);
//        }

//        public static readonly DependencyProperty PasswordProperty =
//            DependencyProperty.Register(
//                nameof(Password),
//                typeof(string),
//                typeof(VolunteerSelfWindow),
//                new PropertyMetadata(string.Empty));

//        public VolunteerSelfWindow(int volunteerId)
//        {
//            InitializeComponent();
//            Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
//            DataContext = this;

//            // Subscribe to observer pattern for automatic updates
//            if (Volunteer != null)
//            {
//                s_bl.Volunteer.AddObserver(Volunteer.Id, RefreshVolunteer);
//            }

//            // Subscribe to window closed event to clean up observer
//            Closed += Window_Closed;
//        }

//        /// <summary>
//        /// Unsubscribes from data updates on window close.
//        /// </summary>
//        private void Window_Closed(object? sender, EventArgs e)
//        {
//            if (Volunteer != null)
//                s_bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteer);
//        }

//        /// <summary>
//        /// Reloads the volunteer details from the business logic layer.
//        /// </summary>
//        private void RefreshVolunteer()
//        {
//            if (Volunteer == null) return;

//            int id = Volunteer.Id;
//            Volunteer = null;
//            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
//            DataContext = null;
//            DataContext = this;
//        }

//        private void btnUpdate_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (Volunteer == null) return;

//                Volunteer.Password = Password;

//                s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
//                MessageBox.Show("הפרטים עודכנו בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);

//                // Clear password after successful update
//                Password = string.Empty;
//                if (PasswordBox != null)
//                    PasswordBox.Clear();

//                // The RefreshVolunteer method will be called automatically by the observer
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("שגיאה בעדכון הפרטים: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// Updates the Password property when the password box is changed.
//        /// </summary>
//        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
//        {
//            var passwordBox = sender as PasswordBox;
//            if (passwordBox != null)
//                Password = passwordBox.Password;
//        }

//        private void btnFinishCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (Volunteer?.callInProgress == null) return;
//            try
//            {
//                s_bl.Call.CompleteCall(Volunteer.Id, Volunteer.callInProgress.Id);
//                MessageBox.Show("הטיפול בקריאה הסתיים.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
//                RefreshVolunteer();
//                // The RefreshVolunteer method will be called automatically by the observer
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("שגיאה בסיום טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (Volunteer?.callInProgress == null) return;
//            try
//            {
//                s_bl.Call.CancelAssignment(Volunteer.Id, Volunteer.callInProgress.Id);
//                MessageBox.Show("הטיפול בקריאה בוטל.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
//                RefreshVolunteer();
//                // The RefreshVolunteer method will be called automatically by the observer
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void btnHistory_Click(object sender, RoutedEventArgs e)
//        {
//            var historyWindow = new VolunteerCallHistoryWindow();
//            historyWindow.Show();
//        }
//        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
//        {
//            var SelectCallForTreatmentWindow = new SelectCallForTreatmentWindow();
//            SelectCallForTreatmentWindow.ShowDialog(); // חוסם עד שהחלון נסגר
//            RefreshVolunteer();
//        }
//    }
//}
using BlApi;
using Microsoft.Web.WebView2.Core;
using PL.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerSelfWindow.xaml
    /// </summary>
    public partial class VolunteerSelfWindow : Window
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();
        private bool _isMapInitialized = false;

        /// <summary>
        /// The volunteer object as a dependency property for proper binding
        /// </summary>
        public BO.Volunteer? Volunteer
        {
            get => (BO.Volunteer?)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register(nameof(Volunteer), typeof(BO.Volunteer), typeof(VolunteerSelfWindow), new PropertyMetadata(null, OnVolunteerChanged));

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
                s_bl.Call.AddObserver(RefreshVolunteer);

            }
          

            // Subscribe to window closed event to clean up observer
            Closed += Window_Closed;

            // Initialize map
            InitializeMapAsync();
        }

        /// <summary>
        /// Initializes the WebView2 map asynchronously.
        /// </summary>
        private async void InitializeMapAsync()
        {
            try
            {
                //לסדר את המפה!!
                //await MapWebView.EnsureCoreWebView2Async();

                //// Load the HTML map
                //var htmlMap = GenerateMapHtml();
                //MapWebView.NavigateToString(htmlMap);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing map: {ex.Message}", "Map Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Generates HTML for the map using OpenStreetMap and Leaflet.
        /// </summary>
        private string GenerateMapHtml()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Emergency Response Map</title>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" />
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
    <style>
        body { margin: 0; padding: 0; }
        #map { height: 250px; width: 100%; }
    </style>
</head>
<body>
    <div id=""map""></div>
    <script>
        // Initialize map centered on Jerusalem
        var map = L.map('map').setView([31.7683, 35.2137], 12);
        
        // Add OpenStreetMap tiles (free)
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
        
        // Store markers for easy access
        var volunteerMarker = null;
        var callMarker = null;
        var routeLine = null;
        
        // Function to clear all markers and lines
        function clearMap() {
            if (volunteerMarker) {
                map.removeLayer(volunteerMarker);
                volunteerMarker = null;
            }
            if (callMarker) {
                map.removeLayer(callMarker);
                callMarker = null;
            }
            if (routeLine) {
                map.removeLayer(routeLine);
                routeLine = null;
            }
        }
        
        // Function to add volunteer marker
        function addVolunteerMarker(lat, lng, name) {
            if (volunteerMarker) {
                map.removeLayer(volunteerMarker);
            }
            
            var blueIcon = L.divIcon({
                html: '<div style=""background-color: blue; width: 12px; height: 12px; border-radius: 50%; border: 2px solid white;""></div>',
                iconSize: [16, 16],
                className: 'volunteer-marker'
            });
            
            volunteerMarker = L.marker([lat, lng], {icon: blueIcon})
                .addTo(map)
                .bindPopup('Your Location<br/>' + name);
        }
        
        // Function to add call marker
        function addCallMarker(lat, lng, description, address) {
            if (callMarker) {
                map.removeLayer(callMarker);
            }
            
            var redIcon = L.divIcon({
                html: '<div style=""background-color: red; width: 12px; height: 12px; border-radius: 50%; border: 2px solid white;""></div>',
                iconSize: [16, 16],
                className: 'call-marker'
            });
            
            callMarker = L.marker([lat, lng], {icon: redIcon})
                .addTo(map)
                .bindPopup('?? Emergency Call<br/>' + description + '<br/>' + address);
        }
        
        // Function to add line between two points
        function addLine(lat1, lng1, lat2, lng2) {
            if (routeLine) {
                map.removeLayer(routeLine);
            }
            
            routeLine = L.polyline([[lat1, lng1], [lat2, lng2]], {
                color: 'red',
                weight: 3,
                opacity: 0.8,
                dashArray: '10, 10'
            }).addTo(map);
        }
        
        // Function to center map on both points
        function centerOnBothPoints(lat1, lng1, lat2, lng2) {
            var group = new L.featureGroup([
                L.marker([lat1, lng1]),
                L.marker([lat2, lng2])
            ]);
            map.fitBounds(group.getBounds().pad(0.1));
        }
        
        // Function to center on single point
        function centerOnPoint(lat, lng, zoom) {
            map.setView([lat, lng], zoom || 14);
        }
        
        // Function called from C# to update map
        function updateMap(volunteerData, callData) {
            clearMap();
            
            if (volunteerData) {
                addVolunteerMarker(volunteerData.lat, volunteerData.lng, volunteerData.name);
                
                if (callData) {
                    addCallMarker(callData.lat, callData.lng, callData.description, callData.address);
                    addLine(volunteerData.lat, volunteerData.lng, callData.lat, callData.lng);
                    centerOnBothPoints(volunteerData.lat, volunteerData.lng, callData.lat, callData.lng);
                } else {
                    centerOnPoint(volunteerData.lat, volunteerData.lng);
                }
            }
        }
    </script>
</body>
</html>";
            return html;
        }

        /// <summary>
        /// Called when the WebView2 navigation is completed.
        /// </summary>
        private void MapWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _isMapInitialized = true;
                UpdateMap();
            }
        }

        /// <summary>
        /// Called when Volunteer property changes - updates the map.
        /// </summary>
        private static void OnVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VolunteerSelfWindow window && window._isMapInitialized)
            {
                window.UpdateMap();
            }
        }

        /// <summary>
        /// Updates the map with volunteer and call locations.
        /// </summary>
        private async void UpdateMap()
        {
            if (!_isMapInitialized || Volunteer == null) return;

            try
            {
                var volunteerData = CreateVolunteerJavaScriptObject();
                var callData = HasCallInProgress ? CreateCallJavaScriptObject() : "null";

                var script = $"updateMap({volunteerData}, {callData});";
                //await MapWebView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating map: {ex.Message}", "Map Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Creates JavaScript object for volunteer data.
        /// </summary>
        private string CreateVolunteerJavaScriptObject()
        {
            if (Volunteer?.Latitude == null || Volunteer?.Longitude == null)
                return "null";

            return $@"{{
                lat: {Volunteer.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                lng: {Volunteer.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                name: '{EscapeJavaScript(Volunteer.FullName ?? "Unknown")}'
            }}";
        }

        /// <summary>
        /// Creates JavaScript object for call data.
        /// </summary>
        private string CreateCallJavaScriptObject()
        {
            var call = s_bl.Call.GetCallDetails(Volunteer?.callInProgress?.Id ?? 0);
            if (call?.Latitude == null || call?.Longitude == null)
                return "null";

            return $@"{{
                lat: {call.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                lng: {call.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                description: '{EscapeJavaScript(call.Description ?? "Emergency")}',
                address: '{EscapeJavaScript(call.FullAddress ?? "Unknown Address")}'
            }}";
        }

        /// <summary>
        /// Escapes JavaScript special characters.
        /// </summary>
        private string EscapeJavaScript(string text)
        {
            return text?.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r") ?? "";
        }

        /// <summary>
        /// Unsubscribes from data updates on window close.
        /// </summary>
        private void Window_Closed(object? sender, EventArgs e)
        {
            if (Volunteer != null)
            {
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteer);
                s_bl.Call.RemoveObserver(RefreshVolunteer);
            }
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

                Password = string.Empty;
                if (PasswordBox != null)
                    PasswordBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון הפרטים: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                //RefreshVolunteer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new VolunteerCallHistoryWindow();
            historyWindow.Show();
        }

        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            var SelectCallForTreatmentWindow = new SelectCallForTreatmentWindow();
            SelectCallForTreatmentWindow.Show();
            //RefreshVolunteer();
        }
    }
}