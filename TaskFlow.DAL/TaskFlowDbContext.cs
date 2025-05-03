using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using TaskFlow.DAL.Models;

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

            // USERS
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Conversion des enums (par defaut en int) en string 
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // TASKS

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // Converter for comments 
            var converter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );

            // Comparer for comments
            var comparer = new ValueComparer<List<string>>(
              (c1, c2) => c1.SequenceEqual(c2),
              c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
              c => c.ToList()
            );

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Comments)
                .HasConversion(converter)
                .Metadata.SetValueComparer(comparer);
        }
    }
}
