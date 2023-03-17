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
    [HttpPost("ChangeRoles")]
    public async Task<ActionResult> ChangeRoles([FromBody] MRoles mRoles)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = _userService.ChangeRoles(currentUser, mRoles.username, mRoles.roles);

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

        var ret = new Dictionary<string, string>
        {
            { "fullName", currentUser.FullName },
            { "title", currentUser.Title }
        };

        return Ok(ret);
    }
}
