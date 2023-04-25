using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Database;
using ProteusWeb.Database.Tables;

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
    public ActionResult Test()
    {
        var user = _userService.GetUser(HttpContext);
        if (user == null)
        {
            return StatusCode(500);
        }
        var role = _userService.GetRole(user);
        if (role == null)
        {
            return StatusCode(500);
        }

        var ret = new UserRole(user, role);

        return Ok(ret);
    }
}