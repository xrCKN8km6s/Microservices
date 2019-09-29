using System.Collections.Generic;

namespace Users.API.Infrastructure
{
    public class User
    {
        public long Id { get; set; }

        public string Sub { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public bool IsActive { get; set; }

        public User()
        {
            UserRoles = new List<UserRole>();
        }
    }
}