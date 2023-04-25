using System.Security.Claims;
using ProteusWeb.Database.Tables;

namespace ProteusWeb.Database;

public class UserService
{
    private readonly DatabaseContext _db;

    public UserService(DatabaseContext db)
    {
        _db = db;
    }

    public List<User> GetAllUsers()
    {
        return _db.Users.ToList();
    }

    public User? GetUser(string username)
    {
        var user = (from u in _db.Users
            where u.Username.Equals(username)
            select u).ToList();

        return user.Count == 0 ? null : user[0];
    }

    public User? GetUser(HttpContext httpContext)
    {
        if (httpContext.User.Identity is not ClaimsIdentity identity) return null;
        var claims = identity.Claims;
        var username = "";
        foreach (var claim in claims)
        {
            if (claim.Type == ClaimTypes.Name)
            {
                username = claim.Value;
            }
        }

        return username == "" ? null : GetUser(username);
    }

    public Role? GetRole(User user)
    {
        var role = (from r in _db.Roles
            join u in _db.Users on r.Id equals u.Id
            where u.Id == user.Id
            select r).ToList();

        return role.Count == 0 ? null : role[0];
    }

    public bool ValidPassword(string username, string passwordHash)
    {
        var user = GetUser(username);
        return user != null && user.PasswordHash.Equals(passwordHash);
    }

    public void SetLoginTime(string username)
    {
        var user = GetUser(username);
        if (user != null)
        {
            user.LastLogin = DateTime.Now;
        }

        _db.SaveChanges();
    }

    public bool HasRole(string username, int roleId)
    {
        var user = GetUser(username);
        if (user == null)
        {
            return false;
        }

        var r = GetRole(user);
        return r != null && r.Id == roleId;
    }

    public bool IsAdministrator(string username)
    {
        return HasRole(username, 1);
    }

    public bool IsEditor(string username)
    {
        return HasRole(username, 2) || IsAdministrator(username);
    }

    public bool RegisterUser(User creator, string username, string passwordHash, string? fullName, string? title,
        string? role)
    {
        if (!IsAdministrator(creator.Username))
        {
            return false;
        }

        if (string.IsNullOrEmpty(fullName))
        {
            fullName = username;
        }

        if (string.IsNullOrEmpty(title))
        {
            title = username;
        }

        if (string.IsNullOrEmpty(role))
        {
            role = "viewer";
        }

        var r = GetRole(role) ?? new Role()
        {
            Id = 3,
            Name = "viewer"
        };

        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            FullName = fullName,
            Title = title,
            RoleId = r.Id
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();
        return true;
    }

    public bool DeleteUser(User creator, string username)
    {
        if (!IsAdministrator(creator.Username))
        {
            return false;
        }

        var user = (from u in _db.Users
            where u.Username == username
            select u).First();

        _db.Users.Remove(user);
        _db.SaveChanges();
        return true;
    }

    public Role? GetRole(string role)
    {
        var r = _db.Roles.Where(r => r.Name == role).ToList();
        return r.Count == 0 ? null : r[0];
    }

    public bool CreateRole(User creator, string role)
    {
        if (!IsAdministrator(creator.Username))
        {
            return false;
        }

        if (GetRole(role) != null)
        {
            return true;
        }

        var newRole = new Role
        {
            Name = role
        };
        _db.Roles.Add(newRole);
        _db.SaveChanges();
        return true;
    }

    public User? ChangeUser(User creator, User user, string? username, string? passwordHash, string? fullName,
        string? title, string? role)
    {
        if (creator.Username != user.Username && !IsAdministrator(creator.Username))
        {
            return null;
        }

        var r = GetRole(user);

        if (!string.IsNullOrEmpty(role) && r?.Name != role)
        {
            r = GetRole(role);
        }
        
        if (r == null)
        {
            return null;
        }

        user.Username = username ?? user.Username;
        user.PasswordHash = passwordHash ?? user.PasswordHash;
        user.FullName = fullName ?? user.FullName;
        user.Title = title ?? user.Title;
        user.RoleId = r.Id;

        _db.SaveChanges();
        return user;
    }

    public bool ChangePassword(User creator, string username, string passwordHash)
    {
        if (creator.Username != username && !IsAdministrator(creator.Username))
        {
            return false;
        }

        var user = GetUser(username);

        if (user == null)
        {
            return false;
        }

        user.PasswordHash = passwordHash;
        _db.SaveChanges();
        return true;
    }

    public List<UserRole>? GetUsers(User creator)
    {
        if (!IsAdministrator(creator.Username))
        {
            return null;
        }
        
        var users = (from u in _db.Users
            join r in _db.Roles on u.Id equals r.Id
            select new UserRole(u, r)).ToList();

        return users;
    }
}