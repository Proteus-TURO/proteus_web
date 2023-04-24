namespace ProteusWeb.Database.Tables;

public class UserRole
{
    public string Username { get; }
    public string Password { get; } = "******";
    public string FullName { get; }
    public string Title { get; }
    public string Role { get; }
    public string LastLogin { get; }

    public UserRole(User user, Role role)
    {
        Username = user.Username;
        FullName = user.FullName;
        Title = user.Title ?? "";
        Role = role.Name;
        LastLogin = user.LastLogin.ToString() ?? "";
    }
}