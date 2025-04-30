using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.DAL.Models;

namespace TaskFlow.Api.Helpers
{
  public class JwtTokenGenerator
  {
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
      var keyString = _configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing from configuration.");
      var issuer = _configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer is missing from configuration.");

      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: issuer,
          claims: claims,
          expires: DateTime.UtcNow.AddHours(2),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
