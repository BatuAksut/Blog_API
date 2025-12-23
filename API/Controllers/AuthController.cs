using API.CustomActionFilters;
using AutoMapper;
using DataAccess.Abstract;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenRepository tokenRepository;
    private readonly IMapper mapper;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, IMapper mapper)
    {
      this.userManager = userManager;
      this.tokenRepository = tokenRepository;
      this.mapper = mapper;
    }

    [HttpPost]
    [ValidateModel]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      var applicationUser = mapper.Map<ApplicationUser>(registerDto);
      //ensure username and email are the same
      applicationUser.UserName = registerDto.Username;
      applicationUser.Email = registerDto.Username;

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

      return Created($"/api/auth/user/{applicationUser.Id}", new { id = applicationUser.Id, message = "User registered successfully" });
    }

    [HttpPost("login")]
    [ValidateModel]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var applicationUser = await userManager.FindByEmailAsync(loginDto.Username);

      if (applicationUser != null && await userManager.CheckPasswordAsync(applicationUser, loginDto.Password))
      {
        var roles = await userManager.GetRolesAsync(applicationUser);

        var jwtToken = tokenRepository.CreateJWTToken(applicationUser, roles.ToList());

        var response = new LoginResponseDto()
        {
          JwtToken = jwtToken
        };

        return Ok(response);
      }

      return BadRequest("Invalid username or password");
    }
  }
}
