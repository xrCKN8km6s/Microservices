namespace Users.API.DTO;

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long[] Roles { get; set; }
}
