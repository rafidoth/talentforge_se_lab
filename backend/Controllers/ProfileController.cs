using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dto;
using server.Entities;
using server.Services.ProfileServices;
using server.Services.UserServices;

namespace server.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController(
    IProfileService profileService,
    IAuthService authService
) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null) return Unauthorized();
        var result = await profileService.GetMeSectionAsync(user.Id);
        return Ok(result);
    }
}
