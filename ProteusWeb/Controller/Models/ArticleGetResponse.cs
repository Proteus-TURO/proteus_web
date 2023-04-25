using System.Globalization;
using ProteusWeb.Database.Tables;

namespace ProteusWeb.Controller.Models;

public class ArticleGetResponse
{
    public string content { get; set; }
    public string creator { get; set; }
    public string createdAt { get; set; }
    public string? lastEditor { get; set; }
    public string? lastEditedAt { get; set; }

    public ArticleGetResponse(Article article, User creator, User? editor)
    {
        content = article.Content;
        this.creator = creator.Username;
        createdAt = article.CreatedAt.ToString(CultureInfo.CurrentCulture);
        lastEditor = editor?.Username;
        lastEditedAt = article.LastChangedAt?.ToString(CultureInfo.CurrentCulture);
    }
}