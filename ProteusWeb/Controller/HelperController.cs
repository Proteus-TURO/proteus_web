using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Helper;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class HelperController : ControllerBase
{
    [HttpGet("GetHash")]
    public string CreateUser(string str)
    {
        return PasswordHelper.CreateHash(str);
    }
}