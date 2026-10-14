using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OfficeAttendance.Services;
using OfficeAttendance.Models;
using Microsoft.Win32;
using System;

namespace OfficeAttendance.Views
{
    public partial class UserManagementView : UserControl
    {
        private readonly UserService userService;
        private readonly User currentUser;

        public UserManagementView(User user)
        {
            InitializeComponent();

            currentUser = user;
            userService = new UserService();

            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource =
                userService.GetAllUsers(currentUser);
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

           ComboBoxItem selectedRole = RoleComboBox.SelectedItem as ComboBoxItem;
            string role = selectedRole?.Content?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageText.Text =
                    "Please fill in all required fields.";

                MessageText.Foreground =
                    new SolidColorBrush(Colors.Red);

                return;
            }

            bool userAdded = userService.AddUser(
                currentUser,
                username,
                password,
                fullName,
                role);

            if (!userAdded)
            {
                MessageText.Text =
                    "Employee could not be added. Check the username, role, and required fields.";

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
            RoleComboBox.SelectedIndex = 0;

            LoadUsers();
        }

        private void ChangeStatusButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button button = (Button)sender;
            User user = (User)button.DataContext;

            bool changed = userService.ChangeUserStatus(
                currentUser,
                user.ID);

            if (!changed)
            {
                MessageText.Text =
                    "Unable to change account status. You cannot change your own status.";

                MessageText.Foreground =
                    new SolidColorBrush(Colors.Red);

                return;
            }

            MessageText.Text =
                "Account status changed successfully.";

            MessageText.Foreground =
                new SolidColorBrush(Colors.Green);

            LoadUsers();
        }


        private void ExportAttendanceButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button button = (Button)sender;
            User employee = (User)button.DataContext;

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Export Employee Attendance",
                FileName = $"Attendance_{employee.Username}_{DateTime.Now:yyyyMMdd}.xlsx",
                DefaultExt = ".xlsx",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                AddExtension = true,
                OverwritePrompt = true
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                AttendanceExportService exportService =
                    new AttendanceExportService();

                exportService.ExportEmployeeAttendance(
                    employee.Username,
                    saveDialog.FileName);

                MessageBox.Show(
                    $"Attendance exported successfully for {employee.FullName}.",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not export attendance.\n\n{ex.Message}",
                    "Export Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



    }
}


