using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TaskFlow.DAL
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
  {
    public TaskFlowDbContext CreateDbContext(string[] args)
    {
      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TaskFlow.Api"))
          .AddJsonFile("appsettings.json")
          .Build();

      var optionsBuilder = new DbContextOptionsBuilder<TaskFlowDbContext>();

      var connectionString = configuration.GetConnectionString("DefaultConnection");

      optionsBuilder.UseSqlServer(connectionString);

      return new TaskFlowDbContext(optionsBuilder.Options);
    }
  }
}
