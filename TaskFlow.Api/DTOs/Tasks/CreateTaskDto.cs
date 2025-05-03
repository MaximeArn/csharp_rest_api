namespace TaskFlow.Api.DTOs.Tasks
{
  public class CreateTaskDto
  {
    public string Title { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
  }
}
