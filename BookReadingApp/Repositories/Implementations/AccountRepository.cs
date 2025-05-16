using BookReadingApp.Models;
using BookReadingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReadingApp.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task<SignInResult> SignInAsync(string email, string password, bool rememberMe)
            => await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
            => await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task SignOutAsync()
            => await _signInManager.SignOutAsync();

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
            => await _userManager.UpdateAsync(user);
    }
}
