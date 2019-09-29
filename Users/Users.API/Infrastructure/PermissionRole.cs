namespace Users.API.Infrastructure
{
    //NOTE: EF Core currently has no support for many-to-many relationships without explicit mapping entity
    public class PermissionRole
    {
        public Permission Permission { get; set; }

        public long RoleId { get; set; }

        public Role Role { get; set; }
    }
}