using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlApi;
using PL.Volunteer;

namespace PL
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }

    public class LoginViewModel : INotifyPropertyChanged
    {
        private static readonly IBl s_bl = BlApi.Factory.Get();

        private string _userId;
        public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    ErrorMessage = "User ID is required.";
                    return;
                }
                App.CurrentUserId = int.Parse(UserId);
                var Volunteer = s_bl.Volunteer.GetVolunteerDetails(App.CurrentUserId);

                var role = s_bl.Volunteer.Login(Volunteer.FullName, Password);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (role == BO.Role.Volunteer)
                    {
                        new VolunteerSelfWindow(int.Parse(UserId)).Show();
                    }
                    else if (role == BO.Role.Manager)
                    {
                        var result = MessageBox.Show("Do you want to enter the management screen?", "Login as Manager",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            new MainWindow().Show();
                        }
                        else
                        {
                            new VolunteerSelfWindow(int.Parse(UserId)).Show();
                        }
                    }
                    Application.Current.Windows[0]?.Close();
                });
            }
            catch (BO.BlDoesNotExistException)
            {
                ErrorMessage = "User not found.";
            }
            catch (BO.BlInvalidFormatException)
            {
                ErrorMessage = "Invalid ID, name or password.";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}