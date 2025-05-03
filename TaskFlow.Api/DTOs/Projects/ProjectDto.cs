using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.DTOs.Tasks;

namespace TaskFlow.Api.DTOs.Projects
{
  public class ProjectDto
  {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime CreationDate { get; set; }

    public UserDto? User { get; set; }
    public List<TaskDto>? Tasks { get; set; }
  }
}