using System;
namespace OfficeAttendance.Models
{
    public class Attendance
    {
        public int ID { get; set;}

        public string Username { get; set;}

        public DateTime Date { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string WorkingHours
        {
            get {
                if (CheckIn == null) {
                    return "-";
                }
                if (CheckOut == null) {
                    return "Working";
                }
                TimeSpan duration = CheckOut.Value - CheckIn.Value;

                return $"{(int )duration.TotalHours}h{duration.TotalMinutes}ms";
            }
        }

    }


}
