using System.Collections.Generic;

namespace Users.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Sub { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public bool IsActive { get; set; }

        public User()
        {
            UserRoles = new List<UserRole>();
        }
    }
}