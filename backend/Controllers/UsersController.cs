using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dto;
using server.Services.UserServices;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserQueryFilter filter)
    {
        var result = await userService.GetUsersAsync(filter);
        return Ok(result);
    }

    [HttpPost("block")]
    public async Task<IActionResult> BlockUser(UserManagementActionDto dto)
    {
        var result = await userService.BlockUserAsync(dto.UserIds);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> UnblockUser(UserManagementActionDto dto)
    {
        var result = await userService.UnblockUserAsync(dto.UserIds);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
