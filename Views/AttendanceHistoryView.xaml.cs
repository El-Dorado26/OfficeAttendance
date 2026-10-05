using System.Linq;
using System.Windows.Controls;
using OfficeAttendance.Data;
using OfficeAttendance.Models;
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
                    attendanceRecords.Select(a => new
                    {
                        EmployeeName =
                            db.Users
                                .Where(u => u.Username == a.Username)
                                .Select(u => u.FullName)
                                .FirstOrDefault()
                            ?? a.Username,

                        a.Date,
                        a.CheckIn,
                        a.CheckOut,
                        a.WorkingHours
                    }).ToList();

                AttendanceDataGrid.ItemsSource =
                    displayRecords;
            }
        }
    }
}
