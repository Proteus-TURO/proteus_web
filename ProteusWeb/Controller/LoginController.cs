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
public class LoginController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public LoginController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] Credentials credentials)
    {
        var isValidUser = _userService.ValidPassword(credentials.username, credentials.password);
        if (!isValidUser) return Unauthorized("username or password incorrect");
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, credentials.username)
        };
        claims.AddRange(_userService.GetUserRoles(credentials.username).Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = _configuration.GetValue<string>("SigningKey");

        if (signingKey == null)
        {
            // TODO: Error
            return StatusCode(500);
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(PasswordHelper.StringToBytes(signingKey)),
                SecurityAlgorithms.HmacSha256Signature)
        );

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });

    }
}