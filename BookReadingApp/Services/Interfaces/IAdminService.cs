using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;

namespace BookReadingApp.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<List<string>> GetUserRolesAsync(ApplicationUser user);
        Task<List<ReportViewModel>> GetAllReportViewModelsAsync();
        Task<Report?> GetReportByBookIdAsync(int bookId);
        Task<bool> AddUserAsync(UserViewModel model);
        Task<bool> UpdateUserAsync(string oldEmail, string newEmail, string fullName, string role);
        Task<bool> DeleteUserAsync(string email);
        Task<bool> CreateReportAsync(int bookId, string reason, string reporterId);
        Task<bool> UpdateReportStatusAsync(int id, string status);

    }
}
