using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.DAL.Models
{
  public class TaskItem
  {
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public TaskProgressStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    public int ProjectId { get; set; }

    public Project? Project { get; set; }

    public List<string> Comments { get; set; } = new();
  }
}
