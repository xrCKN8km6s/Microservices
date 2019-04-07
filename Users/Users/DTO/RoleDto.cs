namespace Users.DTO
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long[] Permissions { get; set; }
        public bool IsGlobal { get; set; }
    }

    public class CreateEditRoleDto
    {
        public string Name { get; set; }
        public long[] Permissions { get; set; }
        public bool IsGlobal { get; set; }
    }
}