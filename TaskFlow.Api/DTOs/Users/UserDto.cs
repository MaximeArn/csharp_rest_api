namespace TaskFlow.Api.DTOs.Users
{
  public class UserDto
  {
    public required int Id { get; set; }
    public required string Email { get; set; }

    public required string Name { get; set; }
  }
}