namespace ProteusWeb.Controller.Models;

public class MCreateUser
{
    public string username { get; set; }
    public string passwordHash { get; set; }
    public string fullName { get; set; }
    public string title { get; set; }
}