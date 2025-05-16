using BookReadingApp.Data;
using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;
using BookReadingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReadingApp.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminRepository(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var list = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add(new UserViewModel
                {
                    Email = user.Email,
                    FullName = user.FullName ?? "Chưa cập nhật",
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return list;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> AddUserAsync(UserViewModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null) return false;

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ProfilePictureUrl = "/uploads/default-avatar.png"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return false;

            var role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;
            await _userManager.AddToRoleAsync(user, role);

            return true;
        }

        public async Task<bool> UpdateUserAsync(string oldEmail, string newEmail, string fullName, string role)
        {
            var user = await _userManager.FindByEmailAsync(oldEmail);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(newEmail))
                user.Email = user.UserName = newEmail;

            user.FullName = fullName;

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRoleAsync(user, role);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        // === BÁO CÁO ===

        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Book)
                .Include(r => r.Reporter)
                .ToListAsync();
        }

        public async Task<Report?> GetReportByBookIdAsync(int bookId)
        {
            return await _context.Reports
                .Include(r => r.Book)
                .Include(r => r.Reporter)
                .FirstOrDefaultAsync(r => r.BookId == bookId);
        }

        public async Task<bool> AddReportAsync(Report report)
        {
            _context.Reports.Add(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, string status)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return false;
            report.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
