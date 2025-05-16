using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;
using BookReadingApp.Repositories.Interfaces;
using BookReadingApp.Services.Interfaces;

namespace BookReadingApp.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            return await _adminRepository.GetAllUsersAsync();
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _adminRepository.GetUserByEmailAsync(email);
        }

        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _adminRepository.GetUserRolesAsync(user);
        }

        public async Task<bool> AddUserAsync(UserViewModel model)
        {
            return await _adminRepository.AddUserAsync(model);
        }

        public async Task<bool> UpdateUserAsync(string oldEmail, string newEmail, string fullName, string role)
        {
            return await _adminRepository.UpdateUserAsync(oldEmail, newEmail, fullName, role);
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            return await _adminRepository.DeleteUserAsync(email);
        }
        public async Task<List<ReportViewModel>> GetAllReportViewModelsAsync()
        {
            var reports = await _adminRepository.GetAllReportsAsync();
            return reports.Select(r => new ReportViewModel
            {
                Id = r.Id,
                BookId = r.BookId,
                BookTitle = r.Book?.Title ?? "Không có tên",
                ReporterName = string.IsNullOrEmpty(r.Reporter?.FullName)
                    ? r.Reporter?.Email?.Split('@')[0] ?? "Ẩn danh"
                    : r.Reporter.FullName,
                Reason = r.Reason,
                ReportedDate = r.ReportedDate,
                Status = r.Status
            }).ToList();
        }

        public Task<Report?> GetReportByBookIdAsync(int bookId)
            => _adminRepository.GetReportByBookIdAsync(bookId);

        public Task<bool> CreateReportAsync(int bookId, string reason, string reporterId)
        {
            var report = new Report
            {
                BookId = bookId,
                ReporterId = reporterId,
                Reason = reason,
                ReportedDate = DateTime.Now,
                Status = "Pending"
            };
            return _adminRepository.AddReportAsync(report);
        }

        public Task<bool> UpdateReportStatusAsync(int id, string status)
            => _adminRepository.UpdateReportStatusAsync(id, status);

    }
}
