using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.DAL.Models
{
  public class Project
  {
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public List<TaskItem> Tasks { get; set; } = new();
  }
}
