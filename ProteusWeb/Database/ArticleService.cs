using System.Security.Claims;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Database.Tables;
using ProteusWeb.Helper;

namespace ProteusWeb.Database;

public class ArticleService
{
    private readonly DatabaseContext _db;
    private readonly UserService _userService;

    public ArticleService(DatabaseContext db, UserService userService)
    {
        _db = db;
        _userService = userService;
    }

    private bool ArticleAlreadyExist(string topic, string title)
    {
        return Enumerable.Any(_db.Articles, article => article.Topic == topic && article.Title == title);
    }

    public bool CreateArticle(string topic, string title, string content, User creator)
    {
        if (!_userService.IsEditor(creator.Username))
        {
            return false;
        }
        
        if (ArticleAlreadyExist(topic, title))
        {
            return false;
        }
        
        var newArticle = new Article
        {
            Title = title,
            Content = content,
            Topic = topic,
            CreatedBy = creator.Id,
            CreatedAt = DateTime.Now
        };

        _db.Articles.Add(newArticle);
        _db.SaveChanges();
        return true;
    }

    public List<Article> GetAllArticles()
    {
        return _db.Articles.ToList();
    }

    public Article? GetArticle(string topic, string title)
    {
        return Enumerable.FirstOrDefault(_db.Articles, article => article.Topic == topic && article.Title == title);
    }
}