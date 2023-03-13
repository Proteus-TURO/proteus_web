using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProteusWeb.Database.Tables;

[Table("Article")]
public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; }

    [Required]
    [Column("created_by")]
    public int CreatedBy { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("last_changed_by")]
    public int LastChangedBy { get; set; }

    [Required]
    [Column("last_changed_at")]
    public DateTime LastChangedAt { get; set; }

    [Required]
    [ForeignKey("Topic")]
    [Column("topic_id")]
    public int TopicId { get; set; }
    
    // [Required]
    // public virtual Topic Topic { get; set; }
}