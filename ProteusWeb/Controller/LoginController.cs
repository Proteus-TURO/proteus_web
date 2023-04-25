using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private const int MinutesUntilExpires = 24 * 60;

    public LoginController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] MCredentials mCredentials)
    {
        var isValidUser = _userService.ValidPassword(mCredentials.username, mCredentials.passwordHash);
        if (!isValidUser) return Unauthorized("username or password incorrect");
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, mCredentials.username)
        };
        var user = _userService.GetUser(mCredentials.username);
        if (user == null)
        {
            // User not in DB
            return Unauthorized("username or password incorrect");
        }
        
        var role = _userService.GetRole(user) ?? new Role
        {
            Id = 3,
            Name = "viewer"
        };
        
        claims.Add(new Claim(ClaimTypes.Role, role.Name));

        var signingKey = _configuration.GetValue<string>("SigningKey");

        if (signingKey == null)
        {
            // TODO: Error
            return StatusCode(500);
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(MinutesUntilExpires),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(PasswordHelper.StringToBytes(signingKey)),
                SecurityAlgorithms.HmacSha256Signature)
        );
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddMinutes(MinutesUntilExpires),
            IsPersistent = true
        };

        var authTicket = new AuthenticationTicket(new ClaimsPrincipal(principal), authProperties,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authTicket.Principal, authTicket.Properties);
        
        _userService.SetLoginTime(mCredentials.username);
        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}