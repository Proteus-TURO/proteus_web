using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProteusWeb.Database.Tables;

[Table("Role")]
public class Role
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")] public string Name { get; set; } = "";
}