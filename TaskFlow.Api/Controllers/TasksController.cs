using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.DAL;
using TaskFlow.DAL.Models;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
  private readonly TaskFlowDbContext _context;

  public TasksController(TaskFlowDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Returns all tasks belonging to the authenticated user's projects.
  /// </summary>
  /// <response code="200">Returns the list of tasks</response>
  [HttpGet]
  public async Task<IActionResult> GetTasks()
  {
    var userId = GetCurrentUserId();

    var tasks = await _context.Tasks
        .Where(t => t.Project != null && t.Project.UserId == userId)
        .Include(t => t.Project)
        .ToListAsync();

    var result = tasks.Select(MapToDto);
    return Ok(result);
  }


  /// <summary>
  /// Gets a specific task by ID.
  /// </summary>
  /// <param name="id">The ID of the task</param>
  /// <response code="200">Returns the task</response>
  /// <response code="404">Task not found or access denied</response>
  [HttpGet("{id}")]
  public async Task<IActionResult> GetTaskById(int id)
  {
    var userId = GetCurrentUserId();

    var task = await _context.Tasks
        .Include(t => t.Project)
        .FirstOrDefaultAsync(t => t.Id == id && t.Project != null && t.Project.UserId == userId);

    if (task == null)
      return NotFound("Task not found or access denied.");

    return Ok(MapToDto(task));
  }


  /// <summary>
  /// Creates a new task in a project owned by the authenticated user.
  /// </summary>
  /// <param name="dto">Task data (title, due date, project ID)</param>
  /// <response code="201">Task successfully created</response>
  /// <response code="400">Project not found or access denied</response>
  [HttpPost]
  public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
  {
    var userId = GetCurrentUserId();

    var project = await _context.Projects
        .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == userId);

    if (project == null)
      return BadRequest("Project not found or access denied.");

    var task = new TaskItem
    {
      Title = dto.Title,
      DueDate = dto.DueDate,
      Status = TaskProgressStatus.Todo,
      ProjectId = dto.ProjectId
    };

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, MapToDto(task));
  }


  /// <summary>
  /// Updates a task if it belongs to the authenticated user's project.
  /// </summary>
  /// <param name="id">The ID of the task to update</param>
  /// <param name="dto">The updated task data (title, due date, status)</param>
  /// <response code="200">Task updated successfully</response>
  /// <response code="404">Task not found or access denied</response>
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
  {
    var userId = GetCurrentUserId();

    if (!Enum.TryParse<TaskProgressStatus>(dto.Status, true, out var parsedStatus) ||
        !Enum.IsDefined(typeof(TaskProgressStatus), parsedStatus))
    {
      return BadRequest($"Invalid status value. Allowed values: {string.Join(", ", Enum.GetNames(typeof(TaskProgressStatus)))}");
    }

    var task = await _context.Tasks
        .Include(t => t.Project)
        .FirstOrDefaultAsync(t => t.Id == id && t.Project != null && t.Project.UserId == userId);

    if (task == null)
      return NotFound("Task not found or access denied.");

    task.Title = dto.Title;
    task.DueDate = dto.DueDate;
    task.Status = parsedStatus;

    await _context.SaveChangesAsync();
    return Ok(MapToDto(task));
  }



  /// <summary>
  /// Deletes a task if it belongs to the authenticated user's project.
  /// </summary>
  /// <param name="id">The ID of the task to delete</param>
  /// <response code="204">Task deleted successfully</response>
  /// <response code="404">Task not found or access denied</response>
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(int id)
  {
    var userId = GetCurrentUserId();

    var task = await _context.Tasks
        .Include(t => t.Project)
        .FirstOrDefaultAsync(t => t.Id == id && t.Project != null && t.Project.UserId == userId);

    if (task == null)
      return NotFound("Task not found or access denied.");

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();
    return NoContent();
  }

  private static TaskDto MapToDto(TaskItem task)
  {
    return new TaskDto
    {
      Id = task.Id,
      Title = task.Title,
      DueDate = task.DueDate,
      Status = task.Status.ToString(),
      ProjectId = task.ProjectId
    };
  }

  private int GetCurrentUserId()
  {
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdStr == null) throw new Exception("User ID not found in token.");
    return int.Parse(userIdStr);
  }
}
