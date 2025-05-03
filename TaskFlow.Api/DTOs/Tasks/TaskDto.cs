namespace TaskFlow.Api.DTOs.Tasks
{
  public class TaskDto
  {
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime? DueDate { get; set; }
    public required string Status { get; set; }
    public required int ProjectId { get; set; }
  }
}