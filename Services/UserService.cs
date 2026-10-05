using System.Collections.Generic;
using System.Linq;
using OfficeAttendance.Data;
using OfficeAttendance.Models;

namespace OfficeAttendance.Services
{
    public class UserService
    {
        public List<User> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.Users
                    .OrderBy(u => u.Username)
                    .ToList();
            }
        }

        public bool AddUser(
            string username,
            string password,
            string fullName,
            string role)
        {
            using (var db = new AppDbContext())
            {
                User existingUser = db.Users
                    .FirstOrDefault(u => u.Username == username);

                if (existingUser != null)
                {
                    return false;
                }

                User user = new User
                {
                    Username = username,
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(password),
                    FullName = fullName,
                    Role = role,
                    IsActive = true
                };

                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
        }
    }
}
