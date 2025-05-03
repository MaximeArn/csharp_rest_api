namespace TaskFlow.Api.DTOs.Tasks
{
  public class UpdateTaskDto
  {
    public string Title { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }

    public List<string>? Comments { get; set; }

  }
}
