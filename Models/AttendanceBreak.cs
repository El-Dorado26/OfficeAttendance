using System;
namespace OfficeAttendance.Models
{
    public class AttendanceBreak
    {
        public int Id { get; set; } //unique ID for the break 

        public int AttendanceId { get; set; } //who's taking the break

        public DateTime BreakStart { get; set; }

        public DateTime? BreakEnd { get; set; }

        public TimeSpan? BreakDuration //automatically calclulates the break length 
        {
            get
            {
                if (BreakEnd == null)
                {
                    return null;
                }

                return BreakEnd.Value - BreakStart;

            }
        }
    }

}
