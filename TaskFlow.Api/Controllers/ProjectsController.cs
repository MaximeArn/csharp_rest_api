using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.DAL;
using TaskFlow.DAL.Models;
using TaskFlow.Api.DTOs.Projects;
using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.DTOs.Tasks;

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
  /// <param name="includeUser">If true, includes project owner's details.</param>
  /// <param name="includeTasks">If true, includes associated tasks.</param>
  /// <response code="200">List of user's projects returned successfully.</response>
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
    var result = projects.Select(p => MapToDto(p, includeUser, includeTasks));
    return Ok(result);
  }



  /// <summary>
  /// Creates a new project for the authenticated user.
  /// </summary>
  /// <param name="dto">Project data (name and optional description).</param>
  /// <response code="201">Project created successfully.</response>
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

    var result = MapToDto(project);
    return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, result);
  }


  /// <summary>
  /// Retrieves a specific project by its ID if owned by the user.
  /// </summary>
  /// <param name="id">The ID of the project.</param>
  /// <param name="includeUser">If true, includes project owner's details.</param>
  /// <param name="includeTasks">If true, includes associated tasks.</param>
  /// <response code="200">The project is returned.</response>
  /// <response code="404">Project not found or not accessible by the user.</response>
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

    var result = MapToDto(project, includeUser, includeTasks);
    return Ok(result);
  }



  /// <summary>
  /// Updates a project owned by the authenticated user.
  /// </summary>
  /// <param name="id">The ID of the project to update.</param>
  /// <param name="dto">Updated project data.</param>
  /// <response code="200">Project updated successfully.</response>
  /// <response code="404">Project not found or not accessible.</response>
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
  {
    var userId = GetCurrentUserId();
    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

    if (project == null)
      return NotFound("Project not found or access denied.");

    project.Name = dto.Name;
    project.Description = dto.Description;

    await _context.SaveChangesAsync();

    var result = MapToDto(project);
    return Ok(result);
  }


  /// <summary>
  /// Deletes a project owned by the authenticated user.
  /// </summary>
  /// <param name="id">The ID of the project to delete.</param>
  /// <response code="204">Project deleted successfully.</response>
  /// <response code="404">Project not found or not accessible.</response>
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteProject(int id)
  {
    var userId = GetCurrentUserId();
    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

    if (project == null)
      return NotFound("Project not found or access denied.");

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();
    return NoContent();
  }

  private static ProjectDto MapToDto(Project p, bool includeUser = false, bool includeTasks = false)
  {
    var dto = new ProjectDto
    {
      Id = p.Id,
      Name = p.Name,
      Description = p.Description,
      CreationDate = p.CreationDate
    };

    if (includeUser && p.User != null)
    {
      dto.User = new UserDto
      {
        Id = p.User.Id,
        Email = p.User.Email,
        Name = p.User.Name
      };
    }

    if (includeTasks && p.Tasks != null)
    {
      dto.Tasks = p.Tasks.Select(t => new TaskDto
      {
        Id = t.Id,
        Title = t.Title,
        DueDate = t.DueDate,
        Status = t.Status.ToString(),
        ProjectId = t.ProjectId,
        Comments = t.Comments ?? new()
      }).ToList();
    }

    return dto;
  }

  private int GetCurrentUserId()
  {
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdStr == null) throw new Exception("User ID not found in token.");
    return int.Parse(userIdStr);
  }
}
