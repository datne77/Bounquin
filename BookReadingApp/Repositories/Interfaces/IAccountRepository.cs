using BookReadingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BookReadingApp.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password);
        Task<SignInResult> SignInAsync(string email, string password, bool rememberMe);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task SignOutAsync();
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    }
}
