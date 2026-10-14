using System.Collections.Generic;
using System.Linq;
using OfficeAttendance.Data;
using OfficeAttendance.Models;

namespace OfficeAttendance.Services
{
    public class UserService
    {
        private bool IsActiveAdmin(
            AppDbContext db,
            User currentUser)
        {
            if (currentUser == null)
            {
                return false;
            }

            User storedUser = db.Users
                .FirstOrDefault(u =>
                    u.ID == currentUser.ID);

            return storedUser != null
                && storedUser.IsActive
                && storedUser.Role == "Admin";
        }

        public List<User> GetAllUsers(User currentUser)
        {
            using (var db = new AppDbContext())
            {
                if (!IsActiveAdmin(db, currentUser))
                {
                    return new List<User>();
                }

                return db.Users
                    .OrderBy(u => u.Username)
                    .ToList();
            }
        }

        public bool AddUser(
            User currentUser,
            string username,
            string password,
            string fullName,
            string role)
        {
            using (var db = new AppDbContext())
            {
                if (!IsActiveAdmin(db, currentUser))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(username)
                    || string.IsNullOrWhiteSpace(password)
                    || string.IsNullOrWhiteSpace(fullName))
                {
                    return false;
                }

                username = username.Trim();
                fullName = fullName.Trim();

                if (role != "Admin" && role != "Employee")
                {
                    return false;
                }

                bool usernameExists = db.Users.Any(u =>
                    u.Username == username);

                if (usernameExists)
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

        public bool ChangeUserStatus(
            User currentUser,
            int userId)
        {
            using (var db = new AppDbContext())
            {
                if (!IsActiveAdmin(db, currentUser))
                {
                    return false;
                }

                User user = db.Users
                    .FirstOrDefault(u => u.ID == userId);

                if (user == null)
                {
                    return false;
                }

                // An administrator cannot deactivate
                // or reactivate their own account here.
                if (user.ID == currentUser.ID)
                {
                    return false;
                }

                user.IsActive = !user.IsActive;

                db.SaveChanges();

                return true;
            }
        }
    }
}

