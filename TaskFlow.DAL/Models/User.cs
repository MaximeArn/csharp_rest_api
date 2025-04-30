using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.DAL.Models
{
  public class User
  {
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    [JsonIgnore]
    public List<Project> Projects { get; set; } = new();
  }
}
