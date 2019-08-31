namespace Users.Infrastructure
{
    //NOTE: EF Core currently has no support for many-to-many relationships without explicit mapping entity
    public class UserRole
    {
        public long UserId { get; set; }

        public User User { get; set; }

        public long RoleId { get; set; }

        public Role Role { get; set; }
    }
}