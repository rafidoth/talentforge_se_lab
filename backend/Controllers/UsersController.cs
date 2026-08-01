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
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(UserManagementActionDto dto)
    {
        var result = await userService.DeleteUserAsync(dto.UserIds);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUsers([FromBody] UserManagementActionDto dto)
    {
        if (string.IsNullOrEmpty(dto.RoleName))
            return BadRequest("RoleName is required.");
            
        var result = await userService.AssignRoleToUserAsync(dto.UserIds, dto.RoleName);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
