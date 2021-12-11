namespace Users.API.DTO;

public class UserProfileDto
{
    public long Id { get; set; }
    public string Sub { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool HasGlobalRole { get; set; }
    public PermissionDto[] Permissions { get; set; }
}
