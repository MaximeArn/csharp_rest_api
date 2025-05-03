using TaskFlow.Api.DTOs.Projects;
using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.Api.DTOs.Users;
using TaskFlow.DAL.Models;

namespace TaskFlow.Api.Mappers;

public static class DtoMappers
{
  public static UserDto MapToDto(User user) => new()
  {
    Id = user.Id,
    Email = user.Email,
    Name = user.Name
  };

  public static TaskDto MapToDto(TaskItem task) => new()
  {
    Id = task.Id,
    Title = task.Title,
    DueDate = task.DueDate,
    Status = task.Status.ToString(),
    ProjectId = task.ProjectId,
    Comments = task.Comments ?? new()
  };

  public static ProjectDto MapToDto(Project project, bool includeUser = false, bool includeTasks = false)
  {
    var dto = new ProjectDto
    {
      Id = project.Id,
      Name = project.Name,
      Description = project.Description,
      CreationDate = project.CreationDate
    };

    if (includeUser && project.User != null)
      dto.User = MapToDto(project.User);

    if (includeTasks && project.Tasks != null)
      dto.Tasks = project.Tasks.Select(MapToDto).ToList();

    return dto;
  }
}
