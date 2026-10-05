using System.Linq;
using System.Windows;
using OfficeAttendance.Data;
using OfficeAttendance.Models;
using BCrypt.Net;
namespace OfficeAttendance
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                CreateDefaultAdmin(db);
            }
        }

        private void CreateDefaultAdmin(AppDbContext db)
        {
            User existingUser = db.Users.FirstOrDefault(u => u.Username == "admin");

            if (existingUser != null)
            {
                return;
            }

            User admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Office Administrator",
                Role = "Admin",
                IsActive = true
            };

            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
}

