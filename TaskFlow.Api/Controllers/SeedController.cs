using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskFlow.DAL;
using TaskFlow.DAL.Models;
using TaskFlow.Api.DTOs.Projects;
using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.DTOs.Tasks;

namespace TaskFlow.Api.Controllers
{
  [ApiController]
  [Route("api/seed")]
  public class SeedController : ControllerBase
  {
    private readonly TaskFlowDbContext _context;

    public SeedController(TaskFlowDbContext context)
    {
      _context = context;
    }

    /// <summary>
    /// Initialise la base avec des données de démonstration.
    /// </summary>
    /// <returns>Un message de confirmation.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SeedDatabase()
    {
      if (_context.Users.Any())
        return BadRequest("Database already seeded.");

      string password = "demo123";
      using var sha = SHA256.Create();
      var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
      string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

      var user = new User
      {
        Name = "Demo User",
        Email = "demo@taskflow.com",
        PasswordHash = hashedPassword,
        Role = UserRole.User
      };

      var project = new Project
      {
        Name = "Projet de démonstration",
        Description = "Un projet test pour l'API",
        CreationDate = DateTime.UtcNow,
        User = user
      };

      var task1 = new TaskItem
      {
        Title = "Tâche 1",
        Status = TaskProgressStatus.Todo,
        DueDate = DateTime.UtcNow.AddDays(3),
        Comments = new List<string> { "Initial comment" },
        Project = project
      };

      var task2 = new TaskItem
      {
        Title = "Tâche 2",
        Status = TaskProgressStatus.InProgress,
        DueDate = DateTime.UtcNow.AddDays(7),
        Comments = new List<string>(),
        Project = project
      };

      await _context.Users.AddAsync(user);
      await _context.Projects.AddAsync(project);
      await _context.Tasks.AddRangeAsync(task1, task2);
      await _context.SaveChangesAsync();

      return Ok("Database seeded with demo user: demo@taskflow.com / demo123");
    }
  }
}
