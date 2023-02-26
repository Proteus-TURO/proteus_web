using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
    public ActionResult Login(string username, string password)
    {
        var isValidUser = _userService.ValidPassword(username, password);
        if (isValidUser)
        {
            var claims = new List<Claim>
            {
                new Claim("username", username)
            };
            claims.AddRange(_userService.GetUserRoles(username).Select(role => new Claim("role", role)));

            var signingKey = _configuration.GetValue<string>("SigningKey");
            
            if (signingKey == null)
            {
                // TODO: Error
                return StatusCode(500);
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(PasswordHelper.StringToBytes(signingKey)), SecurityAlgorithms.HmacSha256Signature)
            );
            
            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        else
        {
            return Unauthorized("user or password incorrect");
        }
    }
}