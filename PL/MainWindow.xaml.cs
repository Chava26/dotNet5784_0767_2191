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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.Closed += MainWindow_Closed;


    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            RiskRange = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"שגיאה בטעינת המסך: {ex.Message}", "שגיאה");
        }
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour);
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day);
    }

    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Month);
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year);
    }
    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskTimeRange(RiskRange);
    }

    private void clockObserver()
    {
        // Update the CurrentTime property by fetching the clock value from the BL
        CurrentTime = s_bl.Admin.GetSystemClock();
    }

    private void configObserver()
    {
        // Update configuration-related dependency properties by fetching values from the BL
        RiskRange = s_bl.Admin.GetRiskTimeRange();
    }

    private void OnHandleCallsButton_Click(object sender, RoutedEventArgs e)
    {
        new CallListWindow().Show();

    }

    private void OnHandleVolunteersButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerListWindow().Show();

    }
    private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("האם אתה בטוח שברצונך לאתחל את בסיס הנתונים?", "אישור אתחול", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all other windows except this one
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                s_bl.Admin.InitializeDatabase();
                MessageBox.Show("בסיס הנתונים אותחל בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה באתחול בסיס הנתונים: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    private void ResetDatabase_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("האם אתה בטוח שברצונך לאפס את בסיס הנתונים?", "אישור איפוס", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all other windows except this one
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                s_bl.Admin.ResetDatabase();
                MessageBox.Show("בסיס הנתונים אופס בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה באיפוס בסיס הנתונים: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

}