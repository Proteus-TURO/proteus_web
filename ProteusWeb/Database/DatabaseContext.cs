using Microsoft.EntityFrameworkCore;
using ProteusWeb.Database.Tables;

namespace ProteusWeb.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Article> Articles { get; set; }
}