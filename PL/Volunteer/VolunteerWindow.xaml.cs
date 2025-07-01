using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window
{
    /// <summary>
    /// Access to the business logic layer.
    /// </summary>
    private readonly IBl _bl = Factory.Get();

    /// <summary>
    /// All available volunteer roles.
    /// </summary>
    public IEnumerable<Role> Roles { get; set; }

    /// <summary>
    /// All available distance measurement types.
    /// </summary>
    public IEnumerable<DistanceType> DistanceTypes { get; set; }

    private readonly bool _isUpdate;
    private readonly int _volunteerId;
    private volatile DispatcherOperation? _observerOperation = null; //stage 7


    /// <summary>
    /// The volunteer object being created or edited.
    /// </summary>
    public BO.Volunteer? Volunteer
    {
        get => (BO.Volunteer?)GetValue(VolunteerProperty);
        set => SetValue(VolunteerProperty, value);
    }

    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register(nameof(Volunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Text displayed on the Add/Update button.
    /// </summary>
    public string ButtonText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public static readonly DependencyProperty ActionTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

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
            typeof(VolunteerWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Updates the Password property when the password box is changed.
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        if (passwordBox != null)
            Password = passwordBox.Password;
    }

    /// <summary>
    /// Constructor for creating or updating a volunteer.
    /// </summary>
    public VolunteerWindow(bool Update = false, int id = 0)
    {
        Roles = Enum.GetValues(typeof(BO.Role)).Cast<Role>();
        DistanceTypes = Enum.GetValues(typeof(BO.DistanceType)).Cast<DistanceType>();

        InitializeComponent();
        Loaded += Window_Loaded;
        Closed += Window_Closed;
        _isUpdate = Update;
        _volunteerId = id;
        ButtonText = Update ? "Update" : "Add";
        DataContext = this;
    }

    /// <summary>
    /// Loads volunteer details from the business layer for update.
    /// </summary>
    private BO.Volunteer? LoadVolunteer(int id)
    {
        var volunteer = _bl.Volunteer.GetVolunteerDetails(id);
        if (volunteer != null)
            return volunteer;

        MessageBox.Show("Volunteer not found.");
        Close();
        return null;
    }

    /// <summary>
    /// Creates a new blank volunteer object.
    /// </summary>
    private static BO.Volunteer CreateNewVolunteer() =>
        new()
        {
            Id = 0,
            FullName = "",
            PhoneNumber = "",
            Email = "",
            Address = "",
            IsActive = false,
            Latitude = 0,
            Longitude = 0,
            MaxDistanceForTask = 0,
            DistanceType = DistanceType.AirDistance,
            role = Role.Volunteer
        };

    /// <summary>
    /// Loads volunteer data on window load depending on mode (add/update).
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_isUpdate)
        {
            Volunteer = LoadVolunteer(_volunteerId);
            if (Volunteer != null)
                _bl.Volunteer.AddObserver(Volunteer.Id, RefreshVolunteer);
        }
        else
        {
            Volunteer = CreateNewVolunteer();
        }
    }

    /// <summary>
    /// Unsubscribes from data updates on window close.
    /// </summary>
    private void Window_Closed(object? sender, EventArgs e)
    {
        if (Volunteer != null && _isUpdate)
            _bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteer);
    }

    /// <summary>
    /// Handles add or update click, validating and saving volunteer data.
    /// </summary>
    private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (Volunteer == null) return;

        try
        {
            Volunteer.Password = Password;

            if (_isUpdate == false)
            {
                _bl.Volunteer.AddVolunteer(Volunteer);
                ShowMessageAndClose("Volunteer added successfully.");
            }
            else
            {
                _bl.Volunteer.UpdateVolunteer(App.CurrentUserId, Volunteer);
                ShowMessageAndClose("Volunteer updated successfully.");
            }
        }
        catch (Exception ex)
        {
            string fullMessage = $"Error: {ex.Message}";
            if (ex.InnerException != null)
            {
                fullMessage += $"\nInner exception: {ex.InnerException.Message}";
            }
            MessageBox.Show(fullMessage);
        }
    }

    /// <summary>
    /// Displays a message and closes the window.
    /// </summary>
    private void ShowMessageAndClose(string message)
    {
        MessageBox.Show(message);
        Password = string.Empty;
        PasswordBox.Clear();
        Close();
    }

    /// <summary>
    /// Reloads the volunteer details from the business logic layer.
    /// </summary>
    private void RefreshVolunteer()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                if (Volunteer == null) return;

                int id = Volunteer.Id;
                Volunteer = null;
                Volunteer = _bl.Volunteer.GetVolunteerDetails(id);
                DataContext = null;
                DataContext = this;
            });
      }

    /// <summary>
    /// Handles selection change in combo box (currently unused).
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}
