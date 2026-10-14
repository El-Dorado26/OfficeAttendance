using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OfficeAttendance.Data;
using OfficeAttendance.Services;

namespace OfficeAttendance.Views
{
    public partial class AttendanceHistoryView : UserControl
    {
        private readonly AttendanceService attendanceService;

        public AttendanceHistoryView()
        {
            InitializeComponent();

            attendanceService = new AttendanceService();

            LoadAttendance();
        }

        private void LoadAttendance()
        {
            using (var db = new AppDbContext())
            {
                var attendanceRecords =
                    attendanceService.GetAllAttendance();

                var displayRecords =
                    attendanceRecords.Select(a =>
                    {
                        TimeSpan? workingTime = null;

                        if (a.CheckIn != null &&
                            a.CheckOut != null)
                        {
                            TimeSpan totalBreakTime =
                                attendanceService.GetTotalBreakDuration(a.ID);

                            workingTime =
                                a.CheckOut.Value -
                                a.CheckIn.Value -
                                totalBreakTime;
                        }

                        return new
                        {
                            EmployeeName =
                                db.Users
                                    .Where(u =>
                                        u.Username == a.Username)
                                    .Select(u => u.FullName)
                                    .FirstOrDefault()
                                ?? a.Username,

                            a.Date,
                            a.CheckIn,
                            a.CheckOut,

                            WorkingHours =
    workingTime != null
        ? attendanceService.FormatWorkingTime(workingTime.Value)
        : a.WorkingHours

                        };
                    })
                    .ToList();

                AttendanceDataGrid.ItemsSource =
                    displayRecords;
            }
        }

        private void SearchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string searchText =
                EmployeeFilterTextBox.Text.Trim();

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

            using (var db = new AppDbContext())
            {
                var attendanceRecords =
                    attendanceService.GetAllAttendance();

                var displayRecords =
                    attendanceRecords.Select(a =>
                    {
                        TimeSpan? workingTime = null;

                        if (a.CheckIn != null &&
                            a.CheckOut != null)
                        {
                            TimeSpan totalBreakTime =
                                attendanceService.GetTotalBreakDuration(a.ID);

                            workingTime =
                                a.CheckOut.Value -
                                a.CheckIn.Value -
                                totalBreakTime;
                        }

                        return new
                        {
                            EmployeeName =
                                db.Users
                                    .Where(u =>
                                        u.Username == a.Username)
                                    .Select(u => u.FullName)
                                    .FirstOrDefault()
                                ?? a.Username,

                            a.Username,
                            a.Date,
                            a.CheckIn,
                            a.CheckOut,

                            WorkingHours =
    workingTime != null
        ? attendanceService.FormatWorkingTime(workingTime.Value)
        : a.WorkingHours

                        };
                    });

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    displayRecords =
                        displayRecords.Where(a =>
                            a.EmployeeName
                                .ToLower()
                                .Contains(searchText.ToLower()) ||
                            a.Username
                                .ToLower()
                                .Contains(searchText.ToLower()));
                }

                if (FromDatePicker.SelectedDate != null)
                {
                    DateTime fromDate =
                        FromDatePicker.SelectedDate.Value.Date;

                    displayRecords =
                        displayRecords.Where(a =>
                            a.Date.Date >= fromDate);
                }

                if (ToDatePicker.SelectedDate != null)
                {
                    DateTime toDate =
                        ToDatePicker.SelectedDate.Value.Date;

                    displayRecords =
                        displayRecords.Where(a =>
                            a.Date.Date <= toDate);
                }

                AttendanceDataGrid.ItemsSource =
                    displayRecords.ToList();
            }
        }

        private void ClearButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            EmployeeFilterTextBox.Text = "";

            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;

            LoadAttendance();
        }
    }
}
