using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window
{
    private readonly IBl _volunteerBl = Factory.Get();

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));


    public IEnumerable<BO.Role> RoleCollection { get; set; }
    public IEnumerable<BO.DistanceType> DistanceTypeCollection { get; set; }

    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register(
            nameof(CurrentVolunteer),
            typeof(BO.Volunteer),
            typeof(VolunteerWindow),
            new PropertyMetadata(null));

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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            _volunteerBl.Volunteer.AddObserver(CurrentVolunteer.Id, RefreshVolunteer);
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            _volunteerBl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteer);
    }
    //private void VolunteerObserver()
    //{
    //    int id = CurrentVolunteer!.Id;
    //    CurrentVolunteer = null;
    //    CurrentVolunteer = _volunteerBl.Volunteer.GetVolunteerDetails(id);
    //}


    public VolunteerWindow(int id = 0)
    {
        InitializeComponent();
        Loaded += Window_Loaded;
        Closed += Window_Closed;
        ButtonText = id != 0 ? "Update" : "Add";



        RoleCollection = Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();
        DistanceTypeCollection = Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        if (id != 0)
        {

            var volunteer = _volunteerBl.Volunteer.GetVolunteerDetails(id);
            if (volunteer != null)
            {
                CurrentVolunteer = volunteer;
            }
            else
            {
                MessageBox.Show("Volunteer not found.");
                Close();
            }
        }
        else
        {
            CurrentVolunteer = new BO.Volunteer
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
                DistanceType = BO.DistanceType.AirDistance,
                role = BO.Role.Volunteer
            };
        }

        DataContext = this;
    }

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer == null)
                return;

            CurrentVolunteer.Password = Password;


            if (CurrentVolunteer.Id == 0)
            {
                _volunteerBl.Volunteer.AddVolunteer(CurrentVolunteer);
                MessageBox.Show("Volunteer added successfully.");


            }
            else
            {
                _volunteerBl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully.");

            }

            Password = "";
            PasswordBox.Clear();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as System.Windows.Controls.PasswordBox;
        if (passwordBox != null)
            Password = passwordBox.Password;
    }
    private void RefreshVolunteer()
    {
        if (CurrentVolunteer == null)
            return;

        int id = CurrentVolunteer.Id;
        CurrentVolunteer = null;
        CurrentVolunteer = _volunteerBl.Volunteer.GetVolunteerDetails(id);
        DataContext = null;
        DataContext = this;
    }

}
