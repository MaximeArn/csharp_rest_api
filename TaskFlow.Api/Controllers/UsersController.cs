using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.Helpers;
using TaskFlow.DAL;
using TaskFlow.DAL.Models;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
  private readonly TaskFlowDbContext _context;
  private readonly JwtTokenGenerator _tokenGenerator;

  public UsersController(TaskFlowDbContext context, JwtTokenGenerator tokenGenerator)
  {
    _context = context;
    _tokenGenerator = tokenGenerator;
  }


  /// <summary>
  /// Registers a new user.
  /// </summary>
  /// <param name="dto">Object containing name, email, and password.</param>
  /// <returns>A confirmation message or an error.</returns>
  /// 
  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto dto)

  {

    if (!new EmailAddressAttribute().IsValid(dto.Email))
      return BadRequest("Invalid email format.");

    if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
      return BadRequest("Password must be at least 6 characters.");

    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
      return BadRequest("Email already in use.");

    var user = new User
    {
      Name = dto.Name,
      Email = dto.Email,
      PasswordHash = ComputeSha256Hash(dto.Password),
      Role = UserRole.User
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok("User registered.");
  }


  /// <summary>
  /// Authenticates a user and returns a JWT token.
  /// </summary>
  /// <param name="dto">Login credentials (email and password).</param>
  /// <returns>A JWT token if credentials are valid, otherwise an error.</returns>
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto dto)
  {
    var hash = ComputeSha256Hash(dto.Password);
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == hash);

    if (user == null)
      return Unauthorized("Invalid credentials.");

    var token = _tokenGenerator.GenerateToken(user);
    return Ok(new { token });
  }

  private string ComputeSha256Hash(string input)
  {
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(bytes).Replace("-", "").ToLower();
  }
}
