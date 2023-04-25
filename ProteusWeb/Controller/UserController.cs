using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Controller.Models;
using ProteusWeb.Database;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }


    //Hoffentlich passt des Killi <3
    [Authorize(Roles = "administrator")]
    [HttpPost("CreateUser")]
    public ActionResult CreateUser([FromBody] MCreateUser mCreateUser)
    {
        // Gibts den Username schon?
        if (_userService.GetUser(mCreateUser.username) != null)
        {
            return BadRequest("The username is already taken");
        }

        // Gerad angemeldeter Benutzer der diese Funktion aufruft
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return StatusCode(500);
        }

        // Neuer Benutzer wird vom currentUser angelegt
        var result = _userService.RegisterUser(currentUser, mCreateUser.username, mCreateUser.passwordHash,
            mCreateUser.fullName, mCreateUser.title, mCreateUser.role);

        return result ? Ok() : StatusCode(500);
    }

    [Authorize(Roles = "administrator")]
    [HttpDelete("DeleteUser")]
    public ActionResult DeleteUser([FromBody] MUsername mUsername)
    {
        if (_userService.GetUser(mUsername.username) == null)
        {
            return BadRequest("The username does not exist");
        }

        if (mUsername.username == "admin")
        {
            return BadRequest("admin is the only user that can not be deleted");
        }

        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return StatusCode(500);
        }

        var result = _userService.DeleteUser(currentUser, mUsername.username);

        return result ? Ok() : StatusCode(500);
    }

    [Authorize]
    [HttpPost("OwnChanges")]
    public ActionResult OwnChanges([FromBody] MUser mUser)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return BadRequest("You have to be logged in");
        }

        if (!string.IsNullOrEmpty(mUser.username))
        {
            var user = _userService.GetUser(mUser.username);
            if (user != null)
            {
                return BadRequest("Username " + mUser.username + "already taken");
            }
        }

        if (!string.IsNullOrEmpty(mUser.role))
        {
            var role = _userService.GetRole(mUser.role);
            if (role == null)
            {
                return BadRequest("Role " + mUser.role + " does not exist");
            }
        }

        var result = _userService.ChangeUser(currentUser, currentUser, mUser.username, mUser.passwordHash,
            mUser.fullName, mUser.title, mUser.role);

        return result != null ? Ok() : StatusCode(500);
    }

    [Authorize(Roles = "administrator")]
    [HttpPost("OthersChanges")]
    public async Task<ActionResult> OthersChanges([FromBody] MOtherUser mUser)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return BadRequest("You have to be logged in");
        }

        var otherUser = _userService.GetUser(mUser.username);
        if (otherUser == null)
        {
            return BadRequest("User with username " + otherUser.Username + " does not exist");
        }

        if (!string.IsNullOrEmpty(mUser.newUsername))
        {
            var user = _userService.GetUser(mUser.newUsername);
            if (user != null)
            {
                return BadRequest("Username " + mUser.newUsername + " already taken");
            }
        }

        if (!string.IsNullOrEmpty(mUser.role))
        {
            var role = _userService.GetRole(mUser.role);
            if (role == null)
            {
                return BadRequest("Role " + mUser.role + " does not exist");
            }
        }

        var result = _userService.ChangeUser(currentUser, otherUser, mUser.newUsername, mUser.passwordHash,
            mUser.fullName, mUser.title, mUser.role);

        return result != null ? Ok() : StatusCode(500);
    }


    [Authorize]
    [HttpGet("GetUserInfo")]
    public async Task<ActionResult> GetUserInfo()
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var role = _userService.GetRole(currentUser);

        var ret = new Dictionary<string, object?>
        {
            { "username", currentUser.Username },
            { "fullName", currentUser.FullName },
            { "title", currentUser.Title },
            { "role", role?.Name }
        };

        return Ok(ret);
    }
    
    [Authorize(Roles = "administrator")]
    [HttpGet("GetUsers")]
    public async Task<ActionResult> GetUsers()
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var users = _userService.GetUsers(currentUser);
        if (users == null)
        {
            return Unauthorized();
        }

        return Ok(users);
    }
}