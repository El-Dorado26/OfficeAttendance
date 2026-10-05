
namespace OfficeAttendance.Models
{
    public class User
    {
        public int ID { get; set; }

        public String Username { get; set; }

        public String PasswordHash { get; set; }

        public String FullName { get; set; }

        public String Role { get; set; }

        public bool IsActive { get; set; }
    }
}
