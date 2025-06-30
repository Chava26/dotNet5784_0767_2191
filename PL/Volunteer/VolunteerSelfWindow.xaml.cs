using BlApi;
using Microsoft.Web.WebView2.Core;
using PL.Call;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerSelfWindow.xaml
    /// </summary>
    public partial class VolunteerSelfWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();
        private bool _isMapInitialized = false;
        private BO.Volunteer? _volunteer;
        private string _password = string.Empty;
        private int? _lastCallId = null; // Track last call ID to detect changes

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The volunteer object with proper property change notification
        /// </summary>
        public BO.Volunteer? Volunteer
        {
            get => _volunteer;
            set
            {
                if (_volunteer != value)
                {
                    var previousCallId = _volunteer?.callInProgress?.Id;
                    _volunteer = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasCallInProgress));
                    OnPropertyChanged(nameof(CanSelectCall));
                    OnPropertyChanged(nameof(CanSetInactive));
                    OnPropertyChanged(nameof(DistanceTypes));

                    // Check if call status changed
                    var currentCallId = _volunteer?.callInProgress?.Id;
                    bool callChanged = previousCallId != currentCallId;

                    // Update map when volunteer changes or call changes
                    if (_isMapInitialized && (callChanged || ShouldUpdateMap(previousCallId, currentCallId)))
                    {
                        UpdateMap();
                    }
                }
            }
        }

        /// <summary>
        /// Determines if map should be updated based on call changes
        /// </summary>
        private bool ShouldUpdateMap(int? previousCallId, int? currentCallId)
        {
            // Update if call was added, removed, or changed
            if (previousCallId != currentCallId) return true;

            // Update if location might have changed
            return true; // For now, always update to be safe
        }

        public static IEnumerable<BO.DistanceType> DistanceTypes => Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        public bool HasCallInProgress => Volunteer?.callInProgress != null;
        public bool CanSelectCall => Volunteer?.callInProgress == null && Volunteer?.IsActive == true;
        public bool CanSetInactive => Volunteer?.callInProgress == null;

        /// <summary>
        /// The password input by the user (bound to password box).
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;

            LoadVolunteerData(volunteerId);

            // Subscribe to observer pattern for automatic updates
            if (Volunteer != null)
            {
                s_bl.Volunteer.AddObserver(Volunteer.Id, OnVolunteerChanged);
                s_bl.Call.AddObserver(OnCallChanged);
            }

            // Subscribe to window closed event to clean up observer
            Closed += Window_Closed;

            // Initialize map
            InitializeMapAsync();
        }

        /// <summary>
        /// Enhanced callback for volunteer changes with better map updating
        /// </summary>
        private void OnVolunteerChanged()
        {
            if (Volunteer == null) return;

            try
            {
                Dispatcher.Invoke(() =>
                {
                    int id = Volunteer.Id;
                    var refreshedVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

                    // Check if location changed
                    bool locationChanged = HasLocationChanged(Volunteer, refreshedVolunteer);

                    Volunteer = refreshedVolunteer; // This will trigger map update via property setter

                    // Force map update if location specifically changed
                    if (locationChanged && _isMapInitialized)
                    {
                        UpdateMapAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error refreshing volunteer data: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                });
            }
        }

        /// <summary>
        /// Enhanced callback for call changes
        /// </summary>
        private void OnCallChanged()
        {
            if (Volunteer == null) return;

            try
            {
                Dispatcher.Invoke(() =>
                {
                    // Refresh volunteer data to get updated call information
                    OnVolunteerChanged();
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error handling call change: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                });
            }
        }

        /// <summary>
        /// Checks if volunteer location has changed
        /// </summary>
        private bool HasLocationChanged(BO.Volunteer? oldVolunteer, BO.Volunteer? newVolunteer)
        {
            if (oldVolunteer == null || newVolunteer == null) return true;

            return oldVolunteer.Latitude != newVolunteer.Latitude ||
                   oldVolunteer.Longitude != newVolunteer.Longitude;
        }

        /// <summary>
        /// Loads volunteer data from business logic layer
        /// </summary>
        private void LoadVolunteerData(int volunteerId)
        {
            try
            {
                Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                _lastCallId = Volunteer?.callInProgress?.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer data: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Initializes the WebView2 map asynchronously.
        /// </summary>
        private async void InitializeMapAsync()
        {
            try
            {
                await MapWebView.EnsureCoreWebView2Async();

                // Load the HTML map
                var htmlMap = GenerateMapHtml();
                MapWebView.NavigateToString(htmlMap);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing map: {ex.Message}", "Map Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Generates HTML for the map using OpenStreetMap and Leaflet.
        /// Enhanced with better visual feedback for updates
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
        body { margin: 0; padding: 0; font-family: Arial, sans-serif; }
        #map { height: 250px; width: 100%; }
        .update-indicator {
            position: absolute;
            top: 10px;
            right: 10px;
            background: rgba(74, 144, 226, 0.9);
            color: white;
            padding: 5px 10px;
            border-radius: 15px;
            font-size: 12px;
            z-index: 1000;
            display: none;
        }
        .volunteer-marker {
            animation: pulse 2s infinite;
        }
        @keyframes pulse {
            0% { transform: scale(1); }
            50% { transform: scale(1.1); }
            100% { transform: scale(1); }
        }
    </style>
</head>
<body>
    <div id=""map""></div>
    <div id=""updateIndicator"" class=""update-indicator"">מעדכן מפה...</div>
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
        var updateIndicator = document.getElementById('updateIndicator');
        
        // Function to show update indicator
        function showUpdateIndicator() {
            updateIndicator.style.display = 'block';
            setTimeout(function() {
                updateIndicator.style.display = 'none';
            }, 1500);
        }
        
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
        
        // Function to add volunteer marker with enhanced styling
        function addVolunteerMarker(lat, lng, name) {
            if (volunteerMarker) {
                map.removeLayer(volunteerMarker);
            }
            
            var blueIcon = L.divIcon({
                html: '<div style=""background-color: #4A90E2; width: 16px; height: 16px; border-radius: 50%; border: 3px solid white; box-shadow: 0 2px 6px rgba(0,0,0,0.4);""></div>',
                iconSize: [22, 22],
                className: 'volunteer-marker'
            });
            
            volunteerMarker = L.marker([lat, lng], {icon: blueIcon})
                .addTo(map)
                .bindPopup('?? המיקום שלך<br/>' + name);
        }
        
        // Function to add call marker with enhanced styling
        function addCallMarker(lat, lng, description, address) {
            if (callMarker) {
                map.removeLayer(callMarker);
            }
            
            var redIcon = L.divIcon({
                html: '<div style=""background-color: #E74C3C; width: 16px; height: 16px; border-radius: 50%; border: 3px solid white; box-shadow: 0 2px 6px rgba(0,0,0,0.4); animation: pulse 2s infinite;""></div>',
                iconSize: [22, 22],
                className: 'call-marker'
            });
            
            callMarker = L.marker([lat, lng], {icon: redIcon})
                .addTo(map)
                .bindPopup('?? קריאת חירום<br/>' + description + '<br/>' + address);
        }
        
        // Function to add line between two points with enhanced styling
        function addLine(lat1, lng1, lat2, lng2) {
            if (routeLine) {
                map.removeLayer(routeLine);
            }
            
            routeLine = L.polyline([[lat1, lng1], [lat2, lng2]], {
                color: '#E74C3C',
                weight: 4,
                opacity: 0.8,
                dashArray: '10, 10'
            }).addTo(map);
            
            // Add distance calculation
            var distance = calculateDistance(lat1, lng1, lat2, lng2);
            var midpoint = [(lat1 + lat2) / 2, (lng1 + lng2) / 2];
            
            L.marker(midpoint, {
                icon: L.divIcon({
                    html: '<div style=""background: rgba(231, 76, 60, 0.9); color: white; padding: 2px 6px; border-radius: 10px; font-size: 11px; white-space: nowrap;"">' + distance.toFixed(1) + ' ק״מ</div>',
                    className: 'distance-marker'
                })
            }).addTo(map);
        }
        
        // Function to calculate distance between two points
        function calculateDistance(lat1, lng1, lat2, lng2) {
            var R = 6371; // Earth's radius in km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLng = (lng2 - lng1) * Math.PI / 180;
            var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                    Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
                    Math.sin(dLng/2) * Math.sin(dLng/2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
            return R * c;
        }
        
        // Function to center map on both points
        function centerOnBothPoints(lat1, lng1, lat2, lng2) {
            var group = new L.featureGroup([
                L.marker([lat1, lng1]),
                L.marker([lat2, lng2])
            ]);
            map.fitBounds(group.getBounds().pad(0.15));
        }
        
        // Function to center on single point
        function centerOnPoint(lat, lng, zoom) {
            map.setView([lat, lng], zoom || 14);
        }
        
        // Enhanced function called from C# to update map
        function updateMap(volunteerData, callData) {
            showUpdateIndicator();
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
                UpdateMapAsync();
            }
        }

        /// <summary>
        /// Updates the map with volunteer and call locations asynchronously.
        /// </summary>
        private async void UpdateMapAsync()
        {
            if (!_isMapInitialized || Volunteer == null) return;

            try
            {
                var volunteerData = CreateVolunteerJavaScriptObject();
                var callData = HasCallInProgress ? CreateCallJavaScriptObject() : "null";

                var script = $"updateMap({volunteerData}, {callData});";
                await MapWebView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating map: {ex.Message}", "Map Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Synchronous version for property setter
        /// </summary>
        private async void UpdateMap()
        {
            await Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(UpdateMapAsync);
            });
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
                name: '{EscapeJavaScript(Volunteer.FullName ?? "לא ידוע")}'
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
                description: '{EscapeJavaScript(call.Description ?? "חירום")}',
                address: '{EscapeJavaScript(call.FullAddress ?? "כתובת לא ידועה")}'
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
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, OnVolunteerChanged);
                s_bl.Call.RemoveObserver(OnCallChanged);
            }
        }

        /// <summary>
        /// Legacy method for backward compatibility
        /// </summary>
        private void RefreshVolunteer()
        {
            OnVolunteerChanged();
        }

        /// <summary>
        /// Raises PropertyChanged event for INotifyPropertyChanged implementation
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer == null)
                {
                    MessageBox.Show("לא נמצאו נתוני מתנדב לעדכון.", "שגיאה",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Store old location for comparison
                var oldLat = Volunteer.Latitude;
                var oldLng = Volunteer.Longitude;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    Volunteer.Password = Password;
                }

                s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה!", "הצלחה",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear password field
                Password = string.Empty;
                if (PasswordBox != null)
                    PasswordBox.Clear();

                // Refresh data and check if location changed
                var refreshedVolunteer = s_bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
                bool locationChanged = oldLat != refreshedVolunteer.Latitude || oldLng != refreshedVolunteer.Longitude;

                Volunteer = refreshedVolunteer;

                // Force map update if location changed
                if (locationChanged && _isMapInitialized)
                {
                    UpdateMapAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בעדכון הפרטים: {ex.Message}", "שגיאה",
                               MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (Volunteer?.callInProgress == null)
            {
                MessageBox.Show("אין קריאה פעילה לסיום.", "מידע",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show("האם אתה בטוח שברצונך לסיים את הטיפול בקריאה זו?",
                                           "אישור סיום קריאה",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    s_bl.Call.CompleteCall(Volunteer.Id, Volunteer.callInProgress.Id);
                    MessageBox.Show("הטיפול בקריאה הושלם בהצלחה.", "הצלחה",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    // Map will be updated automatically via observer pattern
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בסיום הקריאה: {ex.Message}", "שגיאה",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (Volunteer?.callInProgress == null)
            {
                MessageBox.Show("אין קריאה פעילה לביטול.", "מידע",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show("האם אתה בטוח שברצונך לבטל את הטיפול בקריאה זו?",
                                           "אישור ביטול קריאה",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    s_bl.Call.CancelAssignment(Volunteer.Id, Volunteer.callInProgress.Id);
                    MessageBox.Show("הטיפול בקריאה בוטל בהצלחה.", "הצלחה",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    // Map will be updated automatically via observer pattern
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בביטול הקריאה: {ex.Message}", "שגיאה",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var historyWindow = new VolunteerCallHistoryWindow();
                historyWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בפתיחת חלון ההיסטוריה: {ex.Message}", "שגיאה",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CanSelectCall)
                {
                    string message = Volunteer?.IsActive != true
                        ? "לא ניתן לבחור קריאות כאשר המתנדב לא פעיל."
                        : "לא ניתן לבחור קריאה חדשה בזמן טיפול בקריאה קיימת.";

                    MessageBox.Show(message, "לא ניתן לבחור קריאה",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var selectCallWindow = new SelectCallForTreatmentWindow();
                selectCallWindow.Show();

                // Map will be updated automatically via observer pattern when a call is selected
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בפתיחת חלון בחירת הקריאה: {ex.Message}", "שגיאה",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

