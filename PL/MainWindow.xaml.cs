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

namespace PL;

/// <summary>
/// Ultra-modern main control center window for Emergency Road Service system
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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

            // Register observers for automatic updates
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
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
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
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
        Dispatcher.Invoke(() =>
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
        Dispatcher.Invoke(() =>
        {
            RiskRange = s_bl.Admin.GetRiskTimeRange();
        });
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
            ShowSuccessNotification("Service Calls window opened");
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
            ShowSuccessNotification("Technicians window opened");
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
                CloseAllOtherWindows();

                s_bl.Admin.InitializeDatabase();
                ShowSuccessNotification("Database initialized successfully with default data");
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
                    CloseAllOtherWindows();

                    s_bl.Admin.ResetDatabase();
                    ShowSuccessNotification("Database reset successfully. All data has been removed.");
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
    /// Close all application windows except the main window
    /// </summary>
    private void CloseAllOtherWindows()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window != this)
                window.Close();
        }
    }

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