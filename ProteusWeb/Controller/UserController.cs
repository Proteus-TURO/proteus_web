using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Controller.Models;
using ProteusWeb.Database;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly UserService _userService;

    public UserController(DatabaseContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }


    //Hoffentlich passt des Killi <3
    [Authorize(Roles = "administrator")]
    [HttpPost("CreateUser")]
    public async Task<ActionResult> CreateUser([FromBody] MCreateUser mCreateUser)
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
        var result = _userService.RegisterUser(currentUser, mCreateUser.username, mCreateUser.passwordHash, mCreateUser.fullName, mCreateUser.title);

        return result ? Ok() : StatusCode(500);
    }


    [Authorize]
    [HttpPost("ChangeOwnPassword")]
    public async Task<ActionResult> ChangeOwnPassword([FromBody] MPasswordHash mPasswordHash)
    {
        // Nur ein angemeldeter Benutzer kann sein eigenes Passwort �ndern
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return BadRequest("You have to be logged in");
        }

        if (string.IsNullOrEmpty(mPasswordHash.passwordHash))
        {
            return BadRequest("New password is required.");
        }

        var result = _userService.ChangePassword(currentUser, currentUser.Username, mPasswordHash.passwordHash);

        return result ? Ok() : StatusCode(500);
    }

    [Authorize(Roles = "administrator")]
    [HttpPost("ChangeOtherPassword")]
    public async Task<ActionResult> ChangeOtherPassword([FromBody] MCredentials mCredentials)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(mCredentials.passwordHash))
        {
            return BadRequest("New password is required.");
        }

        var result = _userService.ChangePassword(currentUser, mCredentials.username, mCredentials.passwordHash);

        return result ? Ok() : StatusCode(500);
    }
    
    [Authorize(Roles = "administrator")]
    [HttpPost("ChangeRole")]
    public async Task<ActionResult> ChangeRole([FromBody] MRole mRole)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = _userService.ChangeRoles(currentUser, mRole.username, mRole.role);

        return result ? Ok() : StatusCode(500);
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
            { "fullName", currentUser.FullName },
            { "title", currentUser.Title },
            { "role", role?.Name }
        };

        return Ok(ret);
    }
    
    [Authorize]
    [HttpPost("ChangeOwnFullName")]
    public async Task<ActionResult> ChangeOwnFullName([FromBody] MChangeFullName mChangeFullName)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = _userService.ChangeFullName(currentUser, currentUser.Username, mChangeFullName.newName);

        return result ? Ok() : StatusCode(500);
    }
    
    [Authorize]
    [HttpPost("ChangeTitle")]
    public async Task<ActionResult> ChangeTitle([FromBody] MChangeUserTitle mChangeUserTitle)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = _userService.ChangeTitle(currentUser, currentUser.Username, mChangeUserTitle.newTitle);

        return result ? Ok() : StatusCode(500);
    }
}
