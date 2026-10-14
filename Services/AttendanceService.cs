using System;
using System.Linq;
using System.Collections.Generic;
using OfficeAttendance.Data;
using OfficeAttendance.Models;

namespace OfficeAttendance.Services
{
    public class AttendanceService
    {
        
public Attendance GetTodayAttendance(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                return db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();
            }
        }


       
public Attendance CheckIn(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();

                // If today's latest attendance is still active,
                // return it instead of creating another record.
                if (attendance != null && attendance.CheckOut == null)
                {
                    return attendance;
                }

                // No attendance today, or the previous session
                // has been checked out: create a new record.
                Attendance newAttendance = new Attendance
                {
                    Username = username,
                    Date = today,
                    CheckIn = DateTime.Now
                };

                db.Attendances.Add(newAttendance);
                db.SaveChanges();

                return newAttendance;
            }
        }

        
public Attendance CheckOut(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();

                if (attendance == null)
                {
                    return null;
                }

                // Already checked out.
                if (attendance.CheckOut != null)
                {
                    return attendance;
                }

                // Prevent checkout while a break is active.
                AttendanceBreak currentBreak = db.AttendanceBreaks
                    .FirstOrDefault(b =>
                        b.AttendanceId == attendance.ID &&
                        b.BreakEnd == null);

                if (currentBreak != null)
                {
                    return null;
                }

                attendance.CheckOut = DateTime.Now;

                db.SaveChanges();

                return attendance;
            }
        }

        public List<Attendance> GetAllAttendance()
        {
            using (var db = new AppDbContext())
            {
                return db.Attendances.OrderByDescending(a => a.Date).ThenByDescending(a => a.CheckIn).ToList();
            }

        }

       
public AttendanceBreak StartBreak(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();

                if (attendance == null)
                {
                    return null;
                }

                // Cannot start a break after checkout.
                if (attendance.CheckOut != null)
                {
                    return null;
                }

                // Do not start another break if one is already active.
                AttendanceBreak currentBreak =
                    db.AttendanceBreaks.FirstOrDefault(b =>
                        b.AttendanceId == attendance.ID &&
                        b.BreakEnd == null);

                if (currentBreak != null)
                {
                    return currentBreak;
                }

                AttendanceBreak newBreak = new AttendanceBreak
                {
                    AttendanceId = attendance.ID,
                    BreakStart = DateTime.Now
                };

                db.AttendanceBreaks.Add(newBreak);
                db.SaveChanges();

                return newBreak;
            }
        }


        
public AttendanceBreak EndBreak(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();

                if (attendance == null)
                {
                    return null;
                }

                // Cannot end a break after checkout.
                if (attendance.CheckOut != null)
                {
                    return null;
                }

                AttendanceBreak currentBreak =
                    db.AttendanceBreaks.FirstOrDefault(b =>
                        b.AttendanceId == attendance.ID &&
                        b.BreakEnd == null);

                if (currentBreak == null)
                {
                    return null;
                }

                currentBreak.BreakEnd = DateTime.Now;

                db.SaveChanges();

                return currentBreak;
            }
        }


        public TimeSpan GetTotalBreakDuration(int attendanceId)
        {
            using (var db = new AppDbContext())
            {
                var breaks = db.AttendanceBreaks
                    .Where(b =>
                        b.AttendanceId == attendanceId &&
                        b.BreakEnd != null)
                    .ToList();

                TimeSpan totalBreakTime = TimeSpan.Zero;

                foreach (var breakRecord in breaks)
                {
                    totalBreakTime +=
                        breakRecord.BreakEnd.Value -
                        breakRecord.BreakStart;
                }

                return totalBreakTime;
            }
        }


        public AttendanceBreak GetActiveBreak(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .Where(a =>
                        a.Username == username &&
                        a.Date == today)
                    .OrderByDescending(a => a.CheckIn)
                    .FirstOrDefault();

                if (attendance == null || attendance.CheckOut != null)
                {
                    return null;
                }

                return db.AttendanceBreaks.FirstOrDefault(b =>
                    b.AttendanceId == attendance.ID &&
                    b.BreakEnd == null);
            }
        }


        public string FormatWorkingTime(TimeSpan workingTime)
        {
            return $"{(int)workingTime.TotalHours}h " +
                   $"{workingTime.Minutes}m " +
                   $"{workingTime.Seconds}s";
        }


        //temp method to view the relations in visual studio
        public void PrintDatabaseContents()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                var attendances = db.Attendances.ToList();
                var breaks = db.AttendanceBreaks.ToList();

                System.Diagnostics.Debug.WriteLine(
                    "========== USERS ==========");

                foreach (var user in users)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"ID: {user.ID} | " +
                        $"Username: {user.Username} | " +
                        $"Name: {user.FullName} | " +
                        $"Role: {user.Role} | " +
                        $"Active: {user.IsActive}");
                }

                System.Diagnostics.Debug.WriteLine(
                    "========== ATTENDANCES ==========");

                foreach (var attendance in attendances)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"ID: {attendance.ID} | " +
                        $"Username: {attendance.Username} | " +
                        $"Date: {attendance.Date} | " +
                        $"Check In: {attendance.CheckIn} | " +
                        $"Check Out: {attendance.CheckOut}");
                }

                System.Diagnostics.Debug.WriteLine(
                    "========== BREAKS ==========");

                foreach (var breakRecord in breaks)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"ID: {breakRecord.Id} | " +
                        $"Attendance ID: {breakRecord.AttendanceId} | " +
                        $"Start: {breakRecord.BreakStart} | " +
                        $"End: {breakRecord.BreakEnd}");
                }
            }
        }



    }
}

