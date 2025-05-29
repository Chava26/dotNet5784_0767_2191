using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window
{
    private readonly IBl _bl = Factory.Get();

    public IEnumerable<Role> Roles { get; set; }
    public IEnumerable<DistanceType> DistanceTypes { get; set; }
    private readonly bool _isUpdate;
    private readonly int _volunteerId;


    public BO.Volunteer? Volunteer
    {
        get => (BO.Volunteer?)GetValue(VolunteerProperty);
        set => SetValue(VolunteerProperty, value);
    }

    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register(nameof(Volunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public string ButtonText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public static readonly DependencyProperty ActionTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(nameof(Password), typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));

    public VolunteerWindow(bool Update = false,int id=0)
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

    private BO.Volunteer? LoadVolunteer(int id)
    {
        
        var volunteer = _bl.Volunteer.GetVolunteerDetails(id);
        if (volunteer != null)
            return volunteer;

        MessageBox.Show("Volunteer not found.");
        Close();
        return null;
    }

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

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (Volunteer != null && _isUpdate)
            _bl.Volunteer.RemoveObserver(Volunteer!.Id, RefreshVolunteer);
    }

    private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (Volunteer == null) return;

        try
        {
            Volunteer.Password = Password;

            if (_isUpdate ==false)
            {
                _bl.Volunteer.AddVolunteer(Volunteer);
                ShowMessageAndClose("Volunteer added successfully.");
            }
            else
            {
                _bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
                ShowMessageAndClose("Volunteer updated successfully.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void ShowMessageAndClose(string message)
    {
        MessageBox.Show(message);
        Password = string.Empty;
        Close();
    }

    private void RefreshVolunteer()
    {
        if (Volunteer == null) return;

        int id = Volunteer.Id;
        Volunteer = null;
        Volunteer = _bl.Volunteer.GetVolunteerDetails(id);
        DataContext = null;
        DataContext = this;
    }

    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }
}