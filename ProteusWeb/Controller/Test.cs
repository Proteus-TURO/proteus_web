using Microsoft.AspNetCore.Mvc;

namespace ProteusWeb.Controller;

[Route("/test")]
public class Test : ControllerBase
{
    [HttpGet]
    public string Get() => "This is a basic ASP.NET Controller";
}