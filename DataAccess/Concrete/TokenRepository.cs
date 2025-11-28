using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
  public class TokenRepository : ITokenRepository
  {
    private readonly IConfiguration configuration;

    public TokenRepository(IConfiguration configuration)
    {
      this.configuration = configuration;
    }
    public string CreateJWTToken(ApplicationUser user, List<string> roles)
    {
      // FIXME: warnings & expressions that can be simplified.
      // fixed warnings but could not simplify expressions => DONE with C# VSCode extension.
      var claims = new List<Claim>
    {
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()) ,
        new("firstname", user.Firstname ?? string.Empty),
        new("lastname", user.Lastname ?? string.Empty)
    };
      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
          issuer: configuration["Jwt:Issuer"],
          audience: configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(15),
          signingCredentials: credentials
      );
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
