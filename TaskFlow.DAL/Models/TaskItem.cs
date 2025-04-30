using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.DAL.Models
{
  public class TaskItem
  {
    [JsonIgnore]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public TaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }

    [JsonIgnore]
    public int ProjectId { get; set; }

    [JsonIgnore]
    public Project? Project { get; set; }

    public List<string> Comments { get; set; } = new();
  }
}
