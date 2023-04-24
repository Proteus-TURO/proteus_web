using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProteusWeb.Controller.Models;
using ProteusWeb.Database;
using ProteusWeb.Database.Tables;
using ProteusWeb.Helper;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ArticleService _articleService;
    private readonly IConfiguration _configuration;
    private const int MinutesUntilExpires = 24 * 60;

    public ArticleController(UserService userService, ArticleService articleService, IConfiguration configuration)
    {
        _userService = userService;
        _articleService = articleService;
        _configuration = configuration;
    }

    [HttpPost("New")]
    [Authorize(Roles = "editor")]
    public async Task<ActionResult> New([FromBody] MArticlePost mArticlePost)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = _articleService.CreateArticle(mArticlePost.topic, mArticlePost.title, mArticlePost.content, currentUser);

        return result ? Ok() : StatusCode(500);
    }
    
    [HttpGet("GetTitles")]
    [Authorize]
    public async Task<ActionResult> GetTitles()
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null)
        {
            return Unauthorized();
        }

        var articles = _articleService.GetAllArticles();

        var ret = new Dictionary<string, List<string>>();

        foreach (var article in articles)
        {
            if (!ret.ContainsKey(article.Topic))
            {
                ret.Add(article.Topic, new List<string>());
            }
            
            ret[article.Topic].Add(article.Title);
        }

        return Ok(ret);
    }
    
    [HttpGet("GetContent")]
    [Authorize]
    public async Task<ActionResult> GetContent([FromQuery] MGetContent mGetContent)
    {
        var article = _articleService.GetArticle(mGetContent.topic, mGetContent.title);

        if (article == null)
        {
            return BadRequest("Article not found");
        }

        return Ok(article.Content);
    }
    
    [HttpDelete("Delete/{topic}/{title}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromRoute] string topic, [FromRoute] string title)
    {
        var res = _articleService.DeleteArticle(topic, title);

        if (res == false)
        {
            return BadRequest("Article + " + topic + "/" + title + "not found");
        }

        return Ok();
    }
}