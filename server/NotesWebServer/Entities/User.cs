namespace NotesWebServer.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }

    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
}