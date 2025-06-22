using System.Windows;
using System.Windows.Controls;
using BlApi;
//using static BO.Enums;

namespace PL
{
    public partial class AddCallWindow : Window
    {
        private static readonly IBl s_bl = Factory.Get();

        public IEnumerable<BO.CallType> CallTypeCollection =>
            Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

        public AddCallWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var grid = (Grid)this.Content;

                if (grid.Children.Count < 6)
                {
                    MessageBox.Show("����� ���� ������. �� ���� ������ �����.", "�����", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var stack1 = (StackPanel)grid.Children[0]; // ��� �����
                var stack2 = (StackPanel)grid.Children[1]; // �����
                var stack3 = (StackPanel)grid.Children[2]; // �����
                var stack4 = (StackPanel)grid.Children[3]; // ����� �����
                var stack5 = (StackPanel)grid.Children[4]; // ����� ���

                var callTypeComboBox = (ComboBox)stack1.Children[1];
                var descriptionTextBox = (TextBox)stack2.Children[1];
                var addressTextBox = (TextBox)stack3.Children[1];
                var openingDatePicker = (DatePicker)stack4.Children[1];
                var maxFinishDatePicker = (DatePicker)stack5.Children[1];

                var newCall = new BO.Call
                {
                    CallType = (BO.CallType)callTypeComboBox.SelectedItem,
                    Description = descriptionTextBox.Text,
                    FullAddress = addressTextBox.Text,
                    Latitude = null,
                    Longitude = null,
                    OpenTime = DateTime.Now,
                    MaxEndTime = maxFinishDatePicker.SelectedDate ?? DateTime.Now,
                    Status = BO.CallStatus.Open
                };

                s_bl.Call.AddCall(newCall);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}