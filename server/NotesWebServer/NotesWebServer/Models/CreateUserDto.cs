namespace NotesWebServer.Models;

public class CreateUserDto
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    
    public int RoleId { get; set; }
}