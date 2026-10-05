using System.Windows;
using OfficeAttendance.Models;
using OfficeAttendance.Views;

namespace OfficeAttendance
{
    public partial class MainWindow
    {
        private User currentUser;

        public MainWindow()
        {
            InitializeComponent();

            ShowLogin();
        }

        public void ShowLogin()
        {
            currentUser = null;

            Sidebar.Visibility = Visibility.Collapsed;

            MainContent.Children.Clear();
            MainContent.Children.Add(new LoginView());
        }

        public void ShowHome(User user)
        {
            currentUser = user;
            SidebarUserName.Text = user.FullName;
            SidebarUserRole.Text = user.Role;

            Sidebar.Visibility = Visibility.Visible;

            if (user.Role == "Admin")
            {
                AttendanceButton.Visibility = Visibility.Visible;
                UsersButton.Visibility = Visibility.Visible;
            }
            else
            {
                AttendanceButton.Visibility = Visibility.Collapsed;
                UsersButton.Visibility = Visibility.Collapsed;
            }

            MainContent.Children.Clear();
            MainContent.Children.Add(new HomeView(currentUser));
        }

        private void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                return;
            }

            MainContent.Children.Clear();

            MainContent.Children.Add(
                new HomeView(currentUser));
        }

        private void AttendanceButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentUser == null ||
                currentUser.Role != "Admin")
            {
                return;
            }

            MainContent.Children.Clear();

            MainContent.Children.Add(
                new AttendanceHistoryView());
        }

        private void UsersButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentUser == null ||
                currentUser.Role != "Admin")
            {
                return;
            }

            MainContent.Children.Clear();

            MainContent.Children.Add(
                new UserManagementView());
        }

        private void LogoutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowLogin();
        }
    }
}
