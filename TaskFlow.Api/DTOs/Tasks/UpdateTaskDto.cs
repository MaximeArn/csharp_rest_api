namespace TaskFlow.Api.DTOs.Tasks
{
  public class UpdateTaskDto
  {
    public string Title { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public required string Status { get; set; }
  }
}
