using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProteusWeb.Database.Tables;

[Table("User_has_Role")]
[Keyless]
public class UserHasRole
{
    [Key, Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Key, Column("role_id")]
    [ForeignKey("Role")]
    public int RoleId { get; init; }
}