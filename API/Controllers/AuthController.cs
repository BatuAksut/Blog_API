using DataAccess.Abstract;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  // FIXME: the name of the endpoint address must be lowercase.
  // TODO: have a look at this standard guidelines: https://opensource.zalando.com/restful-api-guidelines/#table-of-contents
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenRepository tokenRepository;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
    {
      this.userManager = userManager;
      this.tokenRepository = tokenRepository;
    }



    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      // FIXME: some validation logic? together with mapping logic.
      var applicationUser = new ApplicationUser()
      {
        UserName = registerDto.Username,
        Email = registerDto.Username,
        Firstname = registerDto.Firstname,
        Lastname = registerDto.Lastname
      };
      var identityResult = await userManager.CreateAsync(applicationUser, registerDto.Password);
      if (!identityResult.Succeeded)
      {

        var errors = identityResult.Errors.Select(e => e.Description);
        return BadRequest(new { message = "User creation failed", errors });
      }

      if (registerDto.Roles != null && registerDto.Roles.Any())
      {
        identityResult = await userManager.AddToRolesAsync(applicationUser, registerDto.Roles);
        if (!identityResult.Succeeded)
        {
          var errors = identityResult.Errors.Select(e => e.Description);
          return BadRequest(new { message = "Adding roles failed", errors });
        }
      }

      // FIXME: this is a POST, it creates a record based on an entity. It must return 201 - Created together with the URI of the resource.
      return Ok("User registered successfully");
    }



    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var applicationUser = await userManager.FindByEmailAsync(loginDto.Username);
      if (applicationUser != null && await userManager.CheckPasswordAsync(applicationUser, loginDto.Password))
      {

        var roles = await userManager.GetRolesAsync(applicationUser);
        // [Q]: we are not logging users without any roles? If this is the case, we're returning "Invalid username or password" which is misleading.
        if (roles != null && roles.Count > 0)
        {
          var jwtToken = tokenRepository.CreateJWTToken(applicationUser, roles.ToList());
          var response = new LoginResponseDto()
          {
            JwtToken = jwtToken
          };
          return Ok(response);
        }



      }
      return BadRequest("Invalid username or password");
    }
  }
}
