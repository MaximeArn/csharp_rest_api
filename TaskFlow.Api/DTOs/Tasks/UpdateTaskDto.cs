using TaskFlow.DAL.Models;

namespace TaskFlow.Api.DTOs.Tasks
{
  public class UpdateTaskDto
  {
    public string Title { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public TaskProgressStatus Status { get; set; }
  }
}
