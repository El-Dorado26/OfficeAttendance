using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OfficeAttendance.Models;
using OfficeAttendance.Services;

namespace OfficeAttendance.Views
{
    public partial class MyAttendanceView : UserControl
    {
        private readonly AttendanceService attendanceService;
        private readonly string username;

        public MyAttendanceView(User user)
        {
            InitializeComponent();

            attendanceService = new AttendanceService();

            username = user.Username;

            LoadAttendance();
        }

        private void LoadAttendance()
        {
            var allAttendance =
                attendanceService.GetAllAttendance();

            var myAttendance =
                allAttendance
                    .Where(a => a.Username == username)
                    .Select(a => new
                    {
                        a.Date,
                        a.CheckIn,
                        a.CheckOut,

                        WorkingHours =
                            a.CheckIn != null &&
                            a.CheckOut != null
                                ? CalculateWorkingHours(a)
                                : a.WorkingHours
                    })
                    .ToList();

            AttendanceDataGrid.ItemsSource =
                myAttendance;
        }

        private void SearchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (FromDatePicker.SelectedDate != null &&
                ToDatePicker.SelectedDate != null &&
                FromDatePicker.SelectedDate >
                ToDatePicker.SelectedDate)
            {
                MessageBox.Show(
                    "The From date cannot be later than the To date.",
                    "Invalid Date Range",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var allAttendance =
                attendanceService.GetAllAttendance();

            var myAttendance =
                allAttendance
                    .Where(a => a.Username == username);

            if (FromDatePicker.SelectedDate != null)
            {
                DateTime fromDate =
                    FromDatePicker.SelectedDate.Value.Date;

                myAttendance =
                    myAttendance.Where(a =>
                        a.Date.Date >= fromDate);
            }

            if (ToDatePicker.SelectedDate != null)
            {
                DateTime toDate =
                    ToDatePicker.SelectedDate.Value.Date;

                myAttendance =
                    myAttendance.Where(a =>
                        a.Date.Date <= toDate);
            }

            var displayRecords =
                myAttendance
                    .Select(a => new
                    {
                        a.Date,
                        a.CheckIn,
                        a.CheckOut,

                        WorkingHours =
                            a.CheckIn != null &&
                            a.CheckOut != null
                                ? CalculateWorkingHours(a)
                                : a.WorkingHours
                    })
                    .ToList();

            AttendanceDataGrid.ItemsSource =
                displayRecords;
        }

        private string CalculateWorkingHours(
            Attendance attendance)
        {
            TimeSpan totalBreakTime =
                attendanceService.GetTotalBreakDuration(
                    attendance.ID);

            TimeSpan workingTime =
                attendance.CheckOut.Value -
                attendance.CheckIn.Value -
                totalBreakTime;

            return attendanceService.FormatWorkingTime(workingTime);
        }

        private void ClearButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;

            LoadAttendance();
        }
    }
}

