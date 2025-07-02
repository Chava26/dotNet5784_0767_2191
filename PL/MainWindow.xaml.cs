
using PL.Call;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL;

/// <summary>
/// Ultra-modern main control center window for Emergency Road Service system
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
    private volatile DispatcherOperation? _callsObserverOperation = null; //for calls status updates


    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.Closed += MainWindow_Closed;
    }

    #region Dependency Properties

    /// <summary>
    /// Current system time from business layer
    /// </summary>
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    /// <summary>
    /// Risk time range configuration
    /// </summary>
    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

    /// <summary>
    /// Simulator interval in minutes
    /// </summary>
    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

    /// <summary>
    /// Flag indicating if simulator is currently running
    /// </summary>
    public bool IsSimulatorRunning
    {
        get { return (bool)GetValue(IsSimulatorRunningProperty); }
        set { SetValue(IsSimulatorRunningProperty, value); }
    }

    public static readonly DependencyProperty IsSimulatorRunningProperty =
        DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    #region Call Status Counts Properties

    /// <summary>
    /// Number of calls in Open status
    /// </summary>
    public int OpenCallsCount
    {
        get { return (int)GetValue(OpenCallsCountProperty); }
        set { SetValue(OpenCallsCountProperty, value); }
    }

    public static readonly DependencyProperty OpenCallsCountProperty =
        DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    /// <summary>
    /// Number of calls in InProgress status
    /// </summary>
    public int InProgressCallsCount
    {
        get { return (int)GetValue(InProgressCallsCountProperty); }
        set { SetValue(InProgressCallsCountProperty, value); }
    }

    public static readonly DependencyProperty InProgressCallsCountProperty =
        DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    /// <summary>
    /// Number of calls in Closed status
    /// </summary>
    public int ClosedCallsCount
    {
        get { return (int)GetValue(ClosedCallsCountProperty); }
        set { SetValue(ClosedCallsCountProperty, value); }
    }

    public static readonly DependencyProperty ClosedCallsCountProperty =
        DependencyProperty.Register("ClosedCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    /// <summary>
    /// Number of calls in Expired status
    /// </summary>
    public int ExpiredCallsCount
    {
        get { return (int)GetValue(ExpiredCallsCountProperty); }
        set { SetValue(ExpiredCallsCountProperty, value); }
    }

    public static readonly DependencyProperty ExpiredCallsCountProperty =
        DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    /// <summary>
    /// Number of calls in OpenRisk status
    /// </summary>
    public int OpenRiskCallsCount
    {
        get { return (int)GetValue(OpenRiskCallsCountProperty); }
        set { SetValue(OpenRiskCallsCountProperty, value); }
    }

    public static readonly DependencyProperty OpenRiskCallsCountProperty =
        DependencyProperty.Register("OpenRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    /// <summary>
    /// Number of calls in InProgressRisk status
    /// </summary>
    public int InProgressRiskCallsCount
    {
        get { return (int)GetValue(InProgressRiskCallsCountProperty); }
        set { SetValue(InProgressRiskCallsCountProperty, value); }
    }

    public static readonly DependencyProperty InProgressRiskCallsCountProperty =
        DependencyProperty.Register("InProgressRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    #endregion

    #endregion

    #region Window Lifecycle

    /// <summary>
    /// Initialize window and register observers
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Initialize current values from business layer
            CurrentTime = s_bl.Admin.GetSystemClock();
            RiskRange = s_bl.Admin.GetRiskTimeRange();

            // Initialize call status counts
            UpdateCallStatusCounts();

            // Register observers for automatic updates
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
            s_bl.Call.AddObserver(CallsObserver);
        }
        catch (Exception ex)
        {
            ShowErrorMessage("System Initialization Error", $"Error initializing system: {ex.Message}");
        }
    }

    /// <summary>
    /// Clean up observers when window closes
    /// </summary>
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        // Stop simulator if running before closing
        if (IsSimulatorRunning)
        {
            try
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
            catch (Exception ex)
            {
                // Log error but don't prevent window from closing
                System.Diagnostics.Debug.WriteLine($"Error stopping simulator on window close: {ex.Message}");
            }
        }

        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        s_bl.Call.RemoveObserver(CallsObserver);
    }

    #endregion

    #region Window Control Events

    /// <summary>
    /// Enable window dragging from title bar
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    /// <summary>
    /// Minimize window
    /// </summary>
    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Close application
    /// </summary>
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    #endregion

    #region Observer Methods

    /// <summary>
    /// Observer method for system clock updates
    /// </summary>
    private void ClockObserver()
    {
        // Update the CurrentTime property by fetching the clock value from the BL
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetSystemClock();
            });
    }

    /// <summary>
    /// Observer method for configuration updates
    /// </summary>
    private void ConfigObserver()
    {
        // Update configuration-related dependency properties by fetching values from the BL
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                RiskRange = s_bl.Admin.GetRiskTimeRange();
            });
    }

    /// <summary>
    /// Observer method for calls updates
    /// </summary>
    private void CallsObserver()
    {
        // Update call status counts when calls change
        if (_callsObserverOperation is null || _callsObserverOperation.Status == DispatcherOperationStatus.Completed)
            _callsObserverOperation = Dispatcher.BeginInvoke(() =>
            {
                UpdateCallStatusCounts();
            });
    }

    /// <summary>
    /// Update call status counts from business layer
    /// </summary>
    private void UpdateCallStatusCounts()
    {
        try
        {
            var callQuantities = s_bl.Call.GetCallQuantitiesByStatus().ToArray();

            // The enum values in order: Open=0, InProgress=1, Closed=2, Expired=3, OpenRisk=4, InProgressRisk=5
            OpenCallsCount = callQuantities.Length > 0 ? callQuantities[0] : 0;
            InProgressCallsCount = callQuantities.Length > 1 ? callQuantities[1] : 0;
            ClosedCallsCount = callQuantities.Length > 2 ? callQuantities[2] : 0;
            ExpiredCallsCount = callQuantities.Length > 3 ? callQuantities[3] : 0;
            OpenRiskCallsCount = callQuantities.Length > 4 ? callQuantities[4] : 0;
            InProgressRiskCallsCount = callQuantities.Length > 5 ? callQuantities[5] : 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating call status counts: {ex.Message}");
            // Set default values in case of error
            OpenCallsCount = 0;
            InProgressCallsCount = 0;
            ClosedCallsCount = 0;
            ExpiredCallsCount = 0;
            OpenRiskCallsCount = 0;
            InProgressRiskCallsCount = 0;
        }
    }

    #endregion

    #region Time Control Methods

    /// <summary>
    /// Advance system clock by one minute
    /// </summary>
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
            ShowSuccessNotification("Time advanced by 1 minute");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Time Update Error", $"Error advancing time: {ex.Message}");
        }
    }

    /// <summary>
    /// Advance system clock by one hour
    /// </summary>
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour);
            ShowSuccessNotification("Time advanced by 1 hour");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Time Update Error", $"Error advancing time: {ex.Message}");
        }
    }

    /// <summary>
    /// Advance system clock by one day
    /// </summary>
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day);
            ShowSuccessNotification("Time advanced by 1 day");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Time Update Error", $"Error advancing time: {ex.Message}");
        }
    }

    /// <summary>
    /// Advance system clock by one month
    /// </summary>
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Month);
            ShowSuccessNotification("Time advanced by 1 month");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Time Update Error", $"Error advancing time: {ex.Message}");
        }
    }

    /// <summary>
    /// Advance system clock by one year
    /// </summary>
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year);
            ShowSuccessNotification("Time advanced by 1 year");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Time Update Error", $"Error advancing time: {ex.Message}");
        }
    }

    #endregion

    #region Simulator Methods

    /// <summary>
    /// Toggle simulator start/stop
    /// </summary>
    private void ToggleSimulator_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsSimulatorRunning)
            {
                // Stop simulator
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
                ShowSuccessNotification("Simulator stopped successfully");
            }
            else
            {
                // Start simulator
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
                ShowSuccessNotification($"Simulator started with {Interval} minute interval");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Simulator Error", $"Error toggling simulator: {ex.Message}");
        }
    }

    #endregion

    #region Configuration Methods

    /// <summary>
    /// Update risk time range configuration
    /// </summary>
    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.SetRiskTimeRange(RiskRange);
            ShowSuccessNotification("Risk configuration updated successfully");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Configuration Error", $"Error updating risk time range: {ex.Message}");
        }
    }

    #endregion

    #region Navigation Methods (Card Click Events)

    /// <summary>
    /// Open service calls management window (card click)
    /// </summary>
    private void HandleCalls_Click(object sender, MouseButtonEventArgs e)
    {
        try
        {
            new CallListWindow().Show();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Navigation Error", $"Error opening calls window: {ex.Message}");
        }
    }

    /// <summary>
    /// Open volunteers/technicians management window (card click)
    /// </summary>
    private void HandleVolunteers_Click(object sender, MouseButtonEventArgs e)
    {
        try
        {
            new VolunteerListWindow().Show();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Navigation Error", $"Error opening volunteers window: {ex.Message}");
        }
    }

    #endregion

    #region Database Management Methods (Card Click Events)

    /// <summary>
    /// Initialize database with default data (card click)
    /// </summary>
    private void InitDB_Click(object sender, MouseButtonEventArgs e)
    {
        var result = MessageBox.Show(
            "Initialize Database?\n\nThis will set up the system with default data.",
            "Database Initialization",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                s_bl.Admin.InitializeDatabase();
                ShowSuccessNotification("Database initialized successfully with default data");

                // Update call counts after database initialization
                UpdateCallStatusCounts();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Database Initialization Error", $"An error occurred while initializing the database: {ex.Message}");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    /// <summary>
    /// Reset database to empty state (card click)
    /// </summary>
    private void ResetDB_Click(object sender, MouseButtonEventArgs e)
    {
        var result = MessageBox.Show(
            "Reset Database?\n\nWARNING: This will permanently delete ALL data!",
            "Database Reset",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            // Double confirmation for destructive action
            var confirmResult = MessageBox.Show(
                "This action cannot be undone!\n\nAre you absolutely sure you want to delete all data?",
                "Final Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Stop);

            if (confirmResult == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    s_bl.Admin.ResetDatabase();
                    ShowSuccessNotification("Database reset successfully. All data has been removed.");

                    // Update call counts after database reset
                    UpdateCallStatusCounts();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Database Reset Error", $"An error occurred while resetting the database: {ex.Message}");
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Show error message to user
    /// </summary>
    private void ShowErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Show success message to user
    /// </summary>
    private void ShowSuccessMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Show brief success notification (can be enhanced with toast notifications)
    /// </summary>
    private void ShowSuccessNotification(string message)
    {
        // For now, using MessageBox - can be enhanced with custom toast notifications
        MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion
}