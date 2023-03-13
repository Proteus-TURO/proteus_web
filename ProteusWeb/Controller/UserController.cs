using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProteusWeb.Controller.Models;
using ProteusWeb.Database;
using ProteusWeb.Database.Tables;
using ProteusWeb.Helper;

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
    public async Task<ActionResult> CreateUser(string username, string password)
    {   
        // Gibts den Username schon?
        if (_userService.GetUserByUsername(username) != null)
        {
            return Conflict("The username is already taken");
        }

        // Hash the password
        var hashedPassword = PasswordHelper.createHash(password);

        // Userobjekt wird erstellt
        var newUser = new User
        {
            Username = user.Username,
            PasswordHash = hashedPassword
        };

        // Gerad angemeldeter Benutzer der diese Funktion aufruft
        var currentUser = _userService.GetUser(HttpContext);

        // Neuer Benutzer wird vom currentUser angelegt
        _userService.RegisterUser(currentUser, username, password);

        return Ok();
    }


    [Authorize]
    [HttpPost("ChangeOwnPassword")]
    public async Task<IActionResult> ChangeOwnPassword(string newPassword)
    {
        // Nur ein angemeldeter Benutzer kann sein eigenes Passwort ändern
        var currentUser = _userService.GetUser(HttpContext);

        if (string.IsNullOrEmpty(newPassword))
        {
            return BadRequest("New password is required.");
        }

        // Generiere einen Hash für das neue Passwort
        var newHashedPassword = PasswordHelper.CreateHash(newPassword);

        // Ändere das Passwort des aktuellen Benutzers in der Datenbank
        currentUser.PasswordHash = newHashedPassword;
        _userService.UpdateUser(currentUser);

        return Ok();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("ChangeOtherPassword")]
    public async Task<ActionResult> ChangeOtherPassword(PasswordChangeModel passwordChangeModel)
    {
        // Nur der Admin kann das Passwort eines anderen Benutzers ändern
        // Fügen Sie hier Code hinzu, um das Passwort des Benutzers in der Datenbank zu

    }
