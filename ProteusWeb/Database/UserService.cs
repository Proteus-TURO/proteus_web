using System.Security.Claims;
using LinqToDB;
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

    public bool ValidPassword(string username, string passwordHash)
    {
        var user = GetUser(username);
        return user != null && user.PasswordHash.Equals(passwordHash);
    }

    public List<string> GetUserRoles(string username)
    {
        var user = GetUser(username);
        if (user == null)
        {
            return new List<string>();
        }

        var roles = _db.UserHasRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToList();

        return roles;
    }

    public bool HasRole(string username, string role)
    {
        var roles = GetUserRoles(username);
        return roles.Contains(role);
    }

    public bool IsAdministrator(string username)
    {
        if (HasRole(username, "administrator"))
        {
            return true;
        }

        return false;
    }
    
    public bool IsEditor(string username)
    {
        if (HasRole(username, "editor"))
        {
            return true;
        }

        return false;
    }

    public bool RegisterUser(User creator, string username, string passwordHash)
    {
        if (!IsAdministrator(creator.Username))
        {
            return false;
        }

        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash
        };

        _db.Users.Add(newUser);
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

    public bool AddRoleToUser(User creator, string username, string role)
    {
        if (!IsAdministrator(creator.Username))
        {
            return false;
        }

        var user = GetUser(username);
        if (user == null)
        {
            return false;
        }

        if (GetRole(role) == null)
        {
            if (!CreateRole(creator, role))
            {
                return false;
            }
        }

        var r = GetRole(role);
        if (r == null)
        {
            return false;
        }

        var userHasRole = new UserHasRole
        {
            RoleId = r.Id,
            UserId = user.Id
        };

        _db.UserHasRoles.Add(userHasRole);

        _db.SaveChanges();
        return true;
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
}