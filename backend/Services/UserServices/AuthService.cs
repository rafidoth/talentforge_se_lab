using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using server.Data;
using server.Dto;
using server.Entities;
using server.Exceptions;

namespace server.Services.UserServices;

public class AuthService(SignInManager<ApplicationUser> signInManager, ApplicationDbContext db) : IAuthService
{
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await signInManager.UserManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateNewUserAsync(ApplicationUser user, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return await signInManager.UserManager.CreateAsync(user);
        }
        return await signInManager.UserManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role)
    {
        return await signInManager.UserManager.AddToRoleAsync(user, role);
    }

    public async Task<SignInResult> SignInUserAsync(ApplicationUser user, string password, bool isPersistent)
    {
        if (string.IsNullOrEmpty(password))
        {
            await signInManager.SignInAsync(user, isPersistent);
            return SignInResult.Success;
        }
        return await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure: true);
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Email", "Email and Password are required.");
        }

        var existingUser = await GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Email is already registered. Try to login.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                JoinedAt = DateTime.UtcNow,
                Status = UserStatus.Active,
            };

            var createResult = await CreateNewUserAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }

            var assignRoleResult = await AssignRoleAsync(user, Roles.Candidate);
            if (!assignRoleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(string.Join("; ", assignRoleResult.Errors.Select(e => e.Description)));
            }

            // NOTE: Profile creation excluded temporarily until ProfileService is implemented.
            await transaction.CommitAsync();

            await SignInUserAsync(user, request.Password, isPersistent: false);

            return new RegisterResponse(true, user.Id, Roles.Candidate);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
