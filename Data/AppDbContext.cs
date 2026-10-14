using Microsoft.EntityFrameworkCore;
using OfficeAttendance.Data;
using OfficeAttendance.Models;

namespace OfficeAttendance.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<AttendanceBreak> AttendanceBreaks { get; set; }
        public DbSet<User> Users { get; set; }


        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=attendance.db");
        }*/


        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(
                    @"Data Source=C:\Users\intern.hammad\source\repos\OfficeAttendance\bin\Debug\net10.0-windows\attendance.db");
            }
        }

    }
}
