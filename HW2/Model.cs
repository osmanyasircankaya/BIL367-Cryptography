using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace HW2;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Hash> Hashes { get; set; }

    public string DbPath { get; }

    public ApplicationDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        
        DbPath = Path.Join(path, "homework2.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string PasswordSalt { get; set; }
    public string PasswordHash { get; set; }
}

public class Hash
{
    public int HashId { get; set; }
    public string Text { get; set; }
    public string HashedText { get; set; }
}

