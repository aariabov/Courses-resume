using System.Linq.Expressions;
using Devpull.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Users;

public interface IUserManagerService
{
    Task<AppUser> FindByIdAsync(string userId);
    Task<AppUser> FindByEmailAsync(string email);
    Task<AppUser?> FindByEmailMaybeAsync(string email);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<IdentityResult> CreateAsync(AppUser user, string password);
    Task<IdentityResult> UpdateAsync(AppUser user);
    Task<IdentityResult> ResetPassword(AppUser user, string newPassword);
}

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagerService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppUser> FindByIdAsync(string userId)
    {
        return await GetUser(u => u.Id == userId) ?? throw new NotFoundException(userId);
    }

    public async Task<AppUser> FindByEmailAsync(string email)
    {
        return await GetUser(u => u.Email == email) ?? throw new NotFoundException(email);
    }

    public Task<AppUser?> FindByEmailMaybeAsync(string email)
    {
        return GetUser(u => u.Email == email);
    }

    private Task<AppUser?> GetUser(Expression<Func<AppUser, bool>> filter)
    {
        return _userManager.Users
            .Where(filter)
            .Include(u => u.Payments)
            .Include(u => u.Subscriptions)
            .Include(u => u.RunExercises)
            .FirstOrDefaultAsync();
    }

    public Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }

    public Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> UpdateAsync(AppUser user)
    {
        return _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ResetPassword(AppUser user, string newPassword)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }
}
