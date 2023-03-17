using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProteusWeb.Database.Tables;

[Table("User")]
public class User
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("username")] public string Username { get; set; } = "";

    [Column("password_hash")] public string PasswordHash { get; set; } = "";

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("full_name")] public string FullName { get; set; } = "";

    [Column("title")] public string Title { get; set; } = "";
}