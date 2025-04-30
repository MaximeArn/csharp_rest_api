using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.DAL;
using TaskFlow.DAL.Models;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
  private readonly TaskFlowDbContext _context;

  public ProjectsController(TaskFlowDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Returns all projects owned by the authenticated user.
  /// </summary>
  /// <param name="includeUser">If true, includes the project owner's details.</param>
  /// <param name="includeTasks">If true, includes the list of tasks for each project.</param>
  /// <returns>List of the user's projects.</returns>
  [HttpGet]
  public async Task<IActionResult> GetProjects([FromQuery] bool includeUser = false, [FromQuery] bool includeTasks = false)
  {
    var userId = GetCurrentUserId();
    var query = _context.Projects
        .Where(p => p.UserId == userId)
        .AsQueryable();

    if (includeUser)
      query = query.Include(p => p.User);

    if (includeTasks)
      query = query.Include(p => p.Tasks);

    var projects = await query.ToListAsync();
    return Ok(projects);
  }

  /// <summary>
  /// Creates a new project for the authenticated user.
  /// </summary>
  /// <param name="dto">The project data to create (name and optional description).</param>
  /// <returns>The created project with its ID and metadata.</returns>
  [HttpPost]
  public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
  {
    var userId = GetCurrentUserId();

    var project = new Project
    {
      Name = dto.Name,
      Description = dto.Description,
      CreationDate = DateTime.UtcNow,
      UserId = userId
    };

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
  }

  /// <summary>
  /// Retrieves a specific project by its ID if owned by the authenticated user.
  /// </summary>
  /// <param name="id">The project ID.</param>
  /// <param name="includeUser">If true, includes the project owner's details.</param>
  /// <param name="includeTasks">If true, includes the project's tasks.</param>
  /// <returns>The requested project or an error if not found or unauthorized.</returns>
  [HttpGet("{id}")]
  public async Task<IActionResult> GetProjectById(int id, [FromQuery] bool includeUser = false, [FromQuery] bool includeTasks = false)
  {
    var userId = GetCurrentUserId();

    var query = _context.Projects
        .Where(p => p.Id == id && p.UserId == userId)
        .AsQueryable();

    if (includeUser)
      query = query.Include(p => p.User);

    if (includeTasks)
      query = query.Include(p => p.Tasks);

    var project = await query.FirstOrDefaultAsync();

    if (project == null)
      return NotFound("Project not found or access denied.");

    return Ok(project);
  }

  private int GetCurrentUserId()
  {
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdStr == null) throw new Exception("User ID not found in token.");
    return int.Parse(userIdStr);
  }
}
