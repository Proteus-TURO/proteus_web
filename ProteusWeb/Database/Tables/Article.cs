using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProteusWeb.Database.Tables;

[Table("Article")]
public class Article
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    [Column("title")]
    public string Title { get; set; } = "";

    [Required] 
    [Column("content")] 
    public string Content { get; set; } = "";

    [Required]
    [Column("created_by")]
    [ForeignKey("User")]
    public int CreatedBy { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("last_changed_by")]
    public int? LastChangedBy { get; set; }

    [Column("last_changed_at")]
    public DateTime? LastChangedAt { get; set; }

    [Required]
    [StringLength(256)]
    [Column("topic")]
    public string Topic { get; set; } = "";
}