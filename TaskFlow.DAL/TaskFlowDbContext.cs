using Microsoft.EntityFrameworkCore;
using TaskFlow.DAL.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace TaskFlow.DAL
{
  public class TaskFlowDbContext : DbContext
  {
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
          .IsUnique();

      // Conversion des enums (par defaut en int) en string 
      modelBuilder.Entity<User>()
          .Property(u => u.Role)
          .HasConversion<string>();

      modelBuilder.Entity<TaskItem>()
          .Property(t => t.Status)
          .HasConversion<string>();

      // Comments (List<string>) stockés en JSON dans la base
      var commentsConverter = new ValueConverter<List<string>, string>(
          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
          v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
      );

      modelBuilder.Entity<TaskItem>()
          .Property(t => t.Comments)
          .HasConversion(commentsConverter);
    }
  }
}
