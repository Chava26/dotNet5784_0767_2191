
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlApi;
using PL.Volunteer;

namespace PL
{
    /// <summary>
    /// Ultra-modern login window for Emergency Road Service system
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        /// <summary>
        /// Handle password box changes and update view model
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        /// <summary>
        /// Close the login window
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// View model for the ultra-modern login window
    /// </summary>
    public class LoginViewModel : INotifyPropertyChanged
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();

        private string _userId = string.Empty;
        public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                // Clear status message when error is set
                if (!string.IsNullOrEmpty(value))
                    StatusMessage = string.Empty;
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
                // Clear error message when status is set
                if (!string.IsNullOrEmpty(value))
                    ErrorMessage = string.Empty;
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        /// <summary>
        /// Check if login can be executed
        /// </summary>
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password);
        }

        /// <summary>
        /// Execute login process with enhanced user experience
        /// </summary>
        private void Login()
        {
            try
            {
                // Clear previous messages
                ErrorMessage = string.Empty;
                StatusMessage = "Authenticating user...";

                // Validate input
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    ErrorMessage = "User ID is required";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Password is required";
                    return;
                }

                // Parse and validate User ID
                if (!int.TryParse(UserId, out int userId) || userId <= 0)
                {
                    ErrorMessage = "User ID must be a valid positive number";
                    return;
                }

                // Set current user ID in application
                App.CurrentUserId = userId;

                // Get volunteer details
                StatusMessage = "Retrieving user information...";
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(userId);

                // Attempt login
                StatusMessage = "Verifying credentials...";
                var role = s_bl.Volunteer.Login(volunteer.FullName, Password);

                // Success - handle based on role
                
                    StatusMessage = $"Welcome, {volunteer.FullName}!";

                    if (role == BO.Role.Volunteer)
                    {
                        OpenVolunteerWindow(userId, volunteer.FullName);
                    }
                    else if (role == BO.Role.Manager)
                    {
                        HandleManagerLogin(userId, volunteer.FullName);
                    }
                    else
                    {
                        ErrorMessage = "Unknown user role";
                        return;
                    }

                    // Clear form for next login but keep success message briefly
                    ClearForm();
                
            }
            catch (BO.BlDoesNotExistException)
            {
                ErrorMessage = "User not found in system";
                StatusMessage = string.Empty;
            }
            catch (BO.BlInvalidFormatException)
            {
                ErrorMessage = "Invalid credentials provided";
                StatusMessage = string.Empty;
            }
            catch (System.FormatException)
            {
                ErrorMessage = "User ID must be a valid number";
                StatusMessage = string.Empty;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"System error: {ex.Message}";
                StatusMessage = string.Empty;
            }
        }

        /// <summary>
        /// Open volunteer window for regular volunteers
        /// </summary>
        private void OpenVolunteerWindow(int userId, string fullName)
        {
            try
            {
                var volunteerWindow = new VolunteerSelfWindow(userId);
                volunteerWindow.Show();
                StatusMessage = $"Volunteer dashboard opened for {fullName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error opening volunteer window: {ex.Message}";
            }
        }

        /// <summary>
        /// Handle manager login with choice dialog
        /// </summary>
        private void HandleManagerLogin(int userId, string fullName)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Welcome, {fullName}!\n\nYou have manager privileges. How would you like to proceed?",
                    "Manager Access",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.None);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Open management control center
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        StatusMessage = $"Management control center opened for {fullName}";
                        break;

                    case MessageBoxResult.No:
                        // Open as regular volunteer
                        OpenVolunteerWindow(userId, fullName);
                        break;

                    case MessageBoxResult.Cancel:
                        // User cancelled - do nothing
                        StatusMessage = "Login cancelled by user";
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error handling manager login: {ex.Message}";
            }
        }

        /// <summary>
        /// Clear form fields for next login
        /// </summary>
        private void ClearForm()
        {
            // Clear credentials but keep success message
            UserId = string.Empty;
            Password = string.Empty;

            // Clear password box through application dispatcher
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                var loginWindow = Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault();
                if (loginWindow?.PasswordBox != null)
                {
                    loginWindow.PasswordBox.Clear();
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string?name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Enhanced relay command with CanExecute support
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public event System.EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Manually trigger CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}