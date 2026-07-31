using Microsoft.AspNetCore.Identity;
using server.Dto;
using server.Entities;

namespace server.Services.UserServices;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterDto request);
    Task<LoginResponse> LoginAsync(LoginDto loginDto);
    Task<string> GetUserRoleAsync(ApplicationUser user);
    Task LogoutAsync();
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<IdentityResult> CreateNewUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role);
    Task<SignInResult> SignInUserAsync(ApplicationUser user, string password, bool isPersistent);
}
