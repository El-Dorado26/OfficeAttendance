using System;
using System.Windows;
using System.Windows.Controls;
using OfficeAttendance.Models;
using OfficeAttendance.Services;

namespace OfficeAttendance.Views
{
    public partial class HomeView : UserControl
    {
        private readonly AttendanceService attendanceService;

        private readonly string username;

        public HomeView(User user)
        {
            InitializeComponent();

            WelcomeText.Text = $"Welcome back, {user.FullName}!";

            attendanceService = new AttendanceService();
            username = user.Username;

            LoadTodayAttendance();
        }

        private void LoadTodayAttendance()
        {
            Attendance attendance =
                attendanceService.GetTodayAttendance(username);

            if (attendance == null)
            {
                return;
            }

            if (attendance.CheckIn != null)
            {
                CheckInTimeText.Text =
                    attendance.CheckIn.Value.ToString("hh:mm tt");

                CheckInButton.IsEnabled = false;
                CheckOutButton.IsEnabled = true;

                StatusText.Text =
                    "You are currently working.";
            }

            if (attendance.CheckOut != null)
            {
                CheckOutTimeText.Text =
                    attendance.CheckOut.Value.ToString("hh:mm tt");

                TimeSpan workingTime =
                    attendance.CheckOut.Value -
                    attendance.CheckIn.Value;

                WorkingHoursText.Text =
                    $"{(int)workingTime.TotalHours}h {workingTime.Minutes}m";

                CheckOutButton.IsEnabled = false;

                StatusText.Text =
                    "Your workday has been completed.";
            }
        }

        private void CheckInButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Attendance attendance =
                attendanceService.CheckIn(username);

            CheckInTimeText.Text =
                attendance.CheckIn.Value.ToString("hh:mm tt");

            StatusText.Text =
                "You are currently working.";

            CheckInButton.IsEnabled = false;
            CheckOutButton.IsEnabled = true;
        }

        private void CheckOutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Attendance attendance =
                attendanceService.CheckOut(username);

            if (attendance == null ||
                attendance.CheckIn == null ||
                attendance.CheckOut == null)
            {
                return;
            }

            CheckOutTimeText.Text =
                attendance.CheckOut.Value.ToString("hh:mm tt");

            TimeSpan workingTime =
                attendance.CheckOut.Value -
                attendance.CheckIn.Value;

            WorkingHoursText.Text =
                $"{(int)workingTime.TotalHours}h {workingTime.Minutes}m";

            StatusText.Text =
                "Your workday has been completed.";

            CheckOutButton.IsEnabled = false;
        }
    }
}
