using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OfficeAttendance.Models;
using OfficeAttendance.Services;

namespace OfficeAttendance.Views
{
    public partial class LoginView : UserControl
    {
        private readonly AuthenticationService authenticationService;

        public LoginView()
        {
            InitializeComponent();

            authenticationService = new AuthenticationService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Check if fields are empty
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage.Text =
                    "Please enter your username and password.";

                ErrorMessage.Foreground =
                    new SolidColorBrush(Colors.Red);

                return;
            }

            User user = authenticationService.Login(
                username,
                password);

            if (user != null)
            {
                MainWindow mainWindow =
                    (MainWindow)Application.Current.MainWindow;

                mainWindow.ShowHome(user);
            }
            else
            {
                ErrorMessage.Text =
                    "Invalid username or password.";

                ErrorMessage.Foreground =
                    new SolidColorBrush(Colors.Red);
            }
        }

        private void UsernameTextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            ErrorMessage.Text = "";
        }

        private void PasswordBox_PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
        }
    }
}
