using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OfficeAttendance.Models;
using OfficeAttendance.Services;
using System.Windows.Threading;

namespace OfficeAttendance.Views
{
    public partial class HomeView : UserControl
    {
        private readonly AttendanceService attendanceService;

        private readonly string username;
        private DispatcherTimer breakTimer;
        private DateTime breakStartTime;

        public HomeView(User user)
        {
            InitializeComponent();

            WelcomeText.Text = $"Welcome back, {user.FullName}!";

            attendanceService = new AttendanceService();
            username = user.Username;

            breakTimer = new DispatcherTimer();
            breakTimer.Interval = TimeSpan.FromSeconds(1);

            breakTimer.Tick += BreakTimer_Tick;

            //initial attendance button state
            CheckInButton.IsEnabled = true;
            CheckOutButton.IsEnabled = false;

            LoadTodayAttendance();
            LoadActiveBreak();
        }



        private void BreakTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan breakDuration = DateTime.Now - breakStartTime;
            BreakTimerText.Text = $"Break: {breakDuration:hh\\:mm\\:ss}";
        }

        
private void LoadTodayAttendance()
        {
            Attendance attendance =
                attendanceService.GetTodayAttendance(username);

            if (attendance == null)
            {
                CheckInButton.IsEnabled = true;
                CheckOutButton.IsEnabled = false;
                BreakButton.IsEnabled = false;

                return;
            }

            if (attendance.CheckIn != null)
            {
                CheckInTimeText.Text =
                    attendance.CheckIn.Value.ToString("hh:mm tt");
            }

            if (attendance.CheckOut == null &&
                attendance.CheckIn != null)
            {
                // The employee is currently working.
                CheckInButton.IsEnabled = false;
                BreakButton.IsEnabled = true;
                CheckOutButton.IsEnabled = true;

                StatusText.Text = "You are currently working.";
                return;
            }

            if (attendance.CheckOut != null)
            {
                CheckOutTimeText.Text =
                    attendance.CheckOut.Value.ToString("hh:mm tt");

                TimeSpan totalBreakTime =
                    attendanceService.GetTotalBreakDuration(attendance.ID);

                TimeSpan workingTime =
                    attendance.CheckOut.Value -
                    attendance.CheckIn.Value -
                    totalBreakTime;

                WorkingHoursText.Text =
                    attendanceService.FormatWorkingTime(workingTime);

                // The previous session is complete.
                // Allow another check-in.
                CheckInButton.IsEnabled = true;
                BreakButton.IsEnabled = false;
                CheckOutButton.IsEnabled = false;

                StatusText.Text =
                    "Your previous work session is complete. You can check in again.";
            }
        }


        private void LoadActiveBreak()
        {
            AttendanceBreak activeBreak =
                attendanceService.GetActiveBreak(username);

            if (activeBreak == null)
            {
                return;
            }
            //
            breakStartTime = activeBreak.BreakStart;

            TimeSpan breakDuration =
                DateTime.Now - breakStartTime;

            BreakTimerText.Text =
                $"Break: {breakDuration:hh\\:mm\\:ss}";

            breakTimer.Start();

            BreakButton.Content = "Resume Work";

            CheckOutButton.IsEnabled = false;

            StatusText.Text =
                "You are currently on break.";

            //
        }



        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            Attendance attendance = attendanceService.CheckIn(username);

            if (attendance == null || attendance.CheckIn == null)
            {
                return;
            }

            // Clear times from the previous session.
            CheckOutTimeText.Text = "";
            WorkingHoursText.Text = "";
            BreakTimerText.Text = "";

            // Display the new session's check-in time.
            CheckInTimeText.Text =
                attendance.CheckIn.Value.ToString("hh:mm tt");

            // Reset the dashboard for the new session.
            BreakButton.Content = "Take Break";
            StatusText.Text = "You are currently working.";

            CheckInButton.IsEnabled = false;
            BreakButton.IsEnabled = true;
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

            //new 3
            TimeSpan totalBreakTime =
    attendanceService.GetTotalBreakDuration(attendance.ID);

            TimeSpan workingTime =
                attendance.CheckOut.Value -
                attendance.CheckIn.Value -
                totalBreakTime;

            WorkingHoursText.Text =
                                attendanceService.FormatWorkingTime(workingTime);


            //new 3

            StatusText.Text =
                "Your workday has been completed.";

            CheckOutButton.IsEnabled = false;
            BreakButton.IsEnabled = false;
            breakTimer.Stop();
            BreakTimerText.Text = "";
        }
        private void BreakButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (!breakTimer.IsEnabled)
            {
                AttendanceBreak attendanceBreak =
                    attendanceService.StartBreak(username);

                if (attendanceBreak == null)
                {
                    return;
                }
                
                breakStartTime =
                    attendanceBreak.BreakStart;

                breakTimer.Start();

                BreakButton.Content = "Resume Work";

                CheckOutButton.IsEnabled = false;

                StatusText.Text =
                    "You are currently on break.";

                BreakTimerText.Text =
                    "Break: 00:00:00";
                
            }
            else
            {
                AttendanceBreak attendanceBreak =
                    attendanceService.EndBreak(username);

                if (attendanceBreak == null)
                {
                    return;
                }

                breakTimer.Stop();

                BreakButton.Content = "Take Break";

                CheckOutButton.IsEnabled = true;

                StatusText.Text =
                    "You are currently working.";

                BreakTimerText.Text = "";
            }
        }


    }
}
