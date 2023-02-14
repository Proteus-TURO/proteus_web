using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Database;

namespace ProteusWeb.Controller;

[Route("/test")]
public class Test : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        var db = HttpContext.RequestServices.GetService(typeof(DatabaseController)) as DatabaseController;
        db.GetDatabases();
        return "This is a basic ASP.NET Controller";
    }
}