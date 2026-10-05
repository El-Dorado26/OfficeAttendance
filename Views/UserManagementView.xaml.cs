using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OfficeAttendance.Services;

namespace OfficeAttendance.Views
{
    public partial class UserManagementView : UserControl
    {
        private readonly UserService userService;

        public UserManagementView()
        {
            InitializeComponent();

            userService = new UserService();

            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource =
                userService.GetAllUsers();
        }

        private void AddEmployeeButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string username =
                UsernameTextBox.Text.Trim();

            string fullName =
                FullNameTextBox.Text.Trim();

            string password =
                PasswordBox.Password;

            string role =
                RoleTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageText.Text =
                    "Please fill in all required fields.";

                MessageText.Foreground =
                    new SolidColorBrush(Colors.Red);

                return;
            }
            bool userAdded = userService.AddUser(
                username,
                password,
                fullName,
                role);

            if (!userAdded)
            {

                MessageText.Text = "Username already exists";
            
            MessageText.Foreground =
                new SolidColorBrush(Colors.Red);
            return;
        }

            MessageText.Text =
                "Employee added successfully.";

            MessageText.Foreground =
                new SolidColorBrush(Colors.Green);

            UsernameTextBox.Text = "";
            FullNameTextBox.Text = "";
            PasswordBox.Password = "";

            LoadUsers();
        }
    }
}

