
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using OfficeAttendance.Data;
using OfficeAttendance.Models;
using System.Linq;
using System.Windows;

namespace OfficeAttendance
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                // Apply any pending database migrations.
                db.Database.Migrate();

                // Create the default admin only if one does not exist.
                CreateDefaultAdmin(db);
            }
        }

        private void CreateDefaultAdmin(AppDbContext db)
        {
            User existingUser = db.Users
                .FirstOrDefault(u => u.Username == "admin");

            if (existingUser != null)
            {
                return;
            }

            User admin = new User
            {
                Username = "admin",
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Office Administrator",
                Role = "Admin",
                IsActive = true
            };

            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
}
