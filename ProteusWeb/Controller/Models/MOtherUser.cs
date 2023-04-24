namespace ProteusWeb.Controller.Models;

public class MOtherUser
{
    public string username { get; set; }
    public string? newUsername { get; set; }
    public string? passwordHash { get; set; }
    public string? fullName { get; set; }
    public string? title { get; set; }
    public string? role { get; set; }
}