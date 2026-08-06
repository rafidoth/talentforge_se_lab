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
    
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMeSectionDto dto)
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null) return Unauthorized();
        var result = await profileService.UpdateMeSectionAsync(user, dto);
        return Ok(result);
    }
    
    [HttpPost("attributes")]
    public async Task<IActionResult> AddAttributeToProfile([FromBody] AddProfileAttributeDto dto)
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null) return Unauthorized();
        await profileService.AddAttributeToProfileAsync(user.Id, dto);
        return Ok(new { message = "Attribute added to profile successfully." });
    }

    [HttpPut("attributes")]
    public async Task<IActionResult> UpdateAttributeValueInProfile([FromBody] UpdateProfileAttributeValueDto dto)
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null) return Unauthorized();
        await profileService.UpdateAttributeValueInProfileAsync(user.Id, dto);
        return Ok(new { message = "Attribute updated successfully." });
    }

    [HttpGet("attributes/non-built-in")]
    public async Task<IActionResult> GetNonBuiltInAttributes()
    {
        var user = await authService.GetUserByClaimsPrincipalAsync(User);
        if (user == null) return Unauthorized();
        var attributes = await profileService.GetNonBuiltInAttributesAsync(user.Id);
        return Ok(attributes);
    }
}
