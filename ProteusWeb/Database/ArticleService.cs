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
    
    

    public bool CreateArticle(string title, string content, string topic, User creator)
    {

        return true;
    }
}