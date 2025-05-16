using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;

namespace BookReadingApp.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<List<string>> GetUserRolesAsync(ApplicationUser user);
        Task<List<Report>> GetAllReportsAsync();
        Task<Report?> GetReportByBookIdAsync(int bookId);
        Task<bool> AddUserAsync(UserViewModel model);
        Task<bool> UpdateUserAsync(string oldEmail, string newEmail, string fullName, string role);
        Task<bool> DeleteUserAsync(string email);
        Task<bool> AddReportAsync(Report report);
        Task<bool> UpdateReportStatusAsync(int reportId, string status);

    }
}
