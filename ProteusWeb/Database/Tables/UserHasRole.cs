using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProteusWeb.Database.Tables;

[Table("User_has_Role")]
[Keyless]
public class UserHasRole
{
    [Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Column("role_id")]
    [ForeignKey("Role")]
    public int RoleID { get; set; }

    [Required]
    public virtual User User { get; set; }

    [Required]
    public virtual Role Role { get; set; }
}