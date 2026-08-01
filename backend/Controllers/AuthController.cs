using Microsoft.AspNetCore.Mvc;
using server.Dto;
using server.Services.UserServices;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IUserPreferenceService userPreferenceService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterDto registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var role = await authService.GetUserRoleAsync(user);
        var pref = await userPreferenceService.GetPreferenceAsync(user.Id);
        
        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            role = role ?? "Candidate",
            preference = pref
        });
    }
}
