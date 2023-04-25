using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Controller.Models;
using ProteusWeb.Database;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ArticleService _articleService;

    public ArticleController(UserService userService, ArticleService articleService)
    {
        _userService = userService;
        _articleService = articleService;
    }
    
    [HttpGet("{topic}/{title}")]
    [Authorize]
    public ActionResult GetContent(string topic, string title)
    {
        var article = _articleService.GetArticleGetResponse(topic, title);

        if (article == null)
        {
            return BadRequest("Article not found");
        }

        return Ok(article);
    }

    [HttpPost("{topic}/{title}")]
    [Authorize(Roles = "editor,administrator")]
    public ActionResult New(string topic, string title, [FromBody] MContent mContent)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null || !_userService.IsEditor(currentUser.Username))
        {
            return Unauthorized("Only editors or admins can create an article");
        }

        var result = _articleService.CreateArticle(topic, title, mContent.content, currentUser);

        return result ? Ok() : StatusCode(500);
    }
    
    [HttpPost("{topic}/{title}/UploadFile")]
    [Authorize(Roles = "editor,administrator")]
    public async Task<IActionResult> UploadFile(string topic, string title, IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("Please select a file.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles/private/files", topic, title);
        var fileName = file.FileName;
        var filePath = Path.Combine(folderPath, fileName);

        if (System.IO.File.Exists(filePath))
        {
            var count = 1;
            var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            fileName = $"{fileNameOnly}({count}){extension}";
            filePath = Path.Combine(folderPath, fileName);

            while (System.IO.File.Exists(filePath))
            {
                count++;
                fileName = $"{fileNameOnly}({count}){extension}";
                filePath = Path.Combine(folderPath, fileName);
            }
        }
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var r = HttpContext.Request;
        var url = r.Scheme + "://" + r.Host + "/private/files/" + topic + "/" + title + "/" + fileName;

        return Created(new Uri(url), "Successfully uploaded file");
    }
    
    [HttpPut("{topic}/{title}")]
    [Authorize(Roles = "editor,administrator")]
    public ActionResult Edit(string topic, string title, [FromBody] MContent mContent)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null || !_userService.IsEditor(currentUser.Username))
        {
            return Unauthorized("Only editors or admins can edit an article");
        }

        var result = _articleService.EditArticle(topic, title, mContent.content, currentUser);

        return result ? Ok() : StatusCode(500);
    }
    
    [HttpGet("GetTitles")]
    [Authorize]
    public ActionResult GetTitles()
    {
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

    [HttpDelete("{topic}/{title}")]
    [Authorize]
    public ActionResult Delete([FromRoute] string topic, [FromRoute] string title)
    {
        var currentUser = _userService.GetUser(HttpContext);

        if (currentUser == null || !_userService.IsEditor(currentUser.Username))
        {
            return Unauthorized("Only editors or admins can delete an article");
        }
        
        var res = _articleService.DeleteArticle(topic, title);

        if (res == false)
        {
            return BadRequest("Article + " + topic + "/" + title + "not found");
        }

        return Ok();
    }
}