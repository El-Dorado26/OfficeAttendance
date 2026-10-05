using System.Linq;
using OfficeAttendance.Data;
using OfficeAttendance.Models;

namespace OfficeAttendance.Services
{
    public class AuthenticationService
    {
        public User Login(string username, string password)
        {
            using (var db = new AppDbContext())
            {
                User user = db.Users.FirstOrDefault(u =>
                    u.Username == username &&
                    u.IsActive);

                if (user == null)
                {
                    return null;
                }

                bool passwordIsCorrect =
                    BCrypt.Net.BCrypt.Verify(
                        password,
                        user.PasswordHash);

                if (!passwordIsCorrect)
                {
                    return null;
                }

                return user;
            }
        }
    }
}


