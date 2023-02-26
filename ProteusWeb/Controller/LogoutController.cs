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
public class LogoutController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}