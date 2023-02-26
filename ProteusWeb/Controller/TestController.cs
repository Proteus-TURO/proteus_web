using System.Security.Claims;
using LinqToDB;
using LinqToDB.DataProvider.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProteusWeb.Database;
using ProteusWeb.Database.Tables;
using ProteusWeb.Helper;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class TestController : ControllerBase
{
    private readonly UserService _userService;
    public TestController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public string Get()
    {
        return "This is a basic ASP.NET Controller";
    }
    
    [HttpPost("Authorization")]
    [Authorize]
    public ActionResult Register()
    {
        var user = _userService.GetUser(HttpContext);
        if (user != null)
        {
            return Ok("UserID: " + user.Id);
        }

        return StatusCode(500);
    }
}