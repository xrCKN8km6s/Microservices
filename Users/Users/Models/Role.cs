using System.Collections.Generic;

namespace Users.Models
{
    public class Role
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsGlobal { get; set; }

        public bool IsActive { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<PermissionRole> PermissionRoles { get; set; }

        public Role()
        {
            UserRoles = new List<UserRole>();
            PermissionRoles = new List<PermissionRole>();
        }
    }
}