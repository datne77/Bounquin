using BookReadingApp.Models;
using BookReadingApp.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BookReadingApp.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<bool> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<ProfileViewModel?> GetProfileAsync(string userId);
        Task<ProfileViewModel?> GetDetailsAsync(string userId);
        Task<bool> UpdateProfileAsync(ProfileViewModel model, IFormFile? profilePicture);
    }
}
