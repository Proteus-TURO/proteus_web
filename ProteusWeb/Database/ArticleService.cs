using ProteusWeb.Controller.Models;
using ProteusWeb.Database.Tables;

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
    
    public bool EditArticle(string topic, string title, string content, User creator)
    {
        if (!_userService.IsEditor(creator.Username))
        {
            return false;
        }

        var article = GetArticle(topic, title);
        if (article == null)
        {
            return false;
        }

        article.Content = content;
        article.LastChangedBy = creator.Id;
        article.LastChangedAt = DateTime.Now;

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

    public ArticleGetResponse? GetArticleGetResponse(string topic, string title)
    {
        var article = (from a in _db.Articles
            join creator in _db.Users on a.CreatedBy equals creator.Id
            join editor in _db.Users on a.LastChangedBy equals editor.Id 
            into editors from editor in editors.DefaultIfEmpty()
            where a.Topic == topic
            where a.Title == title
            select new ArticleGetResponse(a, creator, editor)).ToList();
        return article.First();
    }

    public bool DeleteArticle(string topic, string title)
    {
        var article = GetArticle(topic, title);
        if (article == null)
        {
            return false;
        }

        _db.Articles.Remove(article);
        _db.SaveChanges();
        return true;
    }
}