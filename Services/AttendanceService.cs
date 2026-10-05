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
                    .FirstOrDefault(a =>
                        a.Username == username &&
                        a.Date == today);
            }
        }

        public Attendance CheckIn(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .FirstOrDefault(a =>
                        a.Username == username &&
                        a.Date == today);

                if (attendance != null)
                {
                    return attendance;
                }

                attendance = new Attendance
                {
                    Username = username,
                    Date = today,
                    CheckIn = DateTime.Now
                };

                db.Attendances.Add(attendance);
                db.SaveChanges();

                return attendance;
            }
        }

        public Attendance CheckOut(string username)
        {
            using (var db = new AppDbContext())
            {
                DateTime today = DateTime.Today;

                Attendance attendance = db.Attendances
                    .FirstOrDefault(a =>
                        a.Username == username &&
                        a.Date == today);

                if (attendance == null)
                {
                    return null;
                }

                if (attendance.CheckOut != null)
                {
                    return attendance;
                }

                attendance.CheckOut = DateTime.Now;

                db.SaveChanges(); //saving it in the database

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
    }
}

