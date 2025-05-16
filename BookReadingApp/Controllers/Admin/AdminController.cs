using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;
using BookReadingApp.Services.Interfaces;
using BookReadingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReadingApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(
            IAdminService adminService,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo,
            IBookRepository bookRepo
        ) : base(accountRepo, categoryRepo, bookRepo)
        {
            _adminService = adminService;
        }
       
        public IActionResult Dashboard() => View();

        // === NGƯỜI DÙNG ===
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> LoadUserList()
        {
            var users = await _adminService.GetAllUsersAsync();
            return PartialView("Partials/_UserList", users);
        }

        [HttpGet]
        public async Task<IActionResult> LoadEditUserForm()
        {
            var users = await _adminService.GetAllUsersAsync();
            return PartialView("Partials/_EditUserForm", users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo(string email)
        {
            var user = await _adminService.GetUserByEmailAsync(email);
            if (user == null) return BadRequest("Không tìm thấy người dùng.");

            var roles = await _adminService.GetUserRolesAsync(user);
            return PartialView("Partials/_EditUserDetails", new UserViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string OldEmail, string Email, string FullName, string Role)
        {
            var success = await _adminService.UpdateUserAsync(OldEmail, Email, FullName, Role);
            return success
                ? Ok("Cập nhật thành công!")
                : BadRequest("Cập nhật thất bại hoặc không tìm thấy người dùng.");
        }

        [HttpGet]
        public async Task<IActionResult> LoadDeleteUserForm()
        {
            var users = await _adminService.GetAllUsersAsync();
            return PartialView("Partials/_DeleteUserForm", users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var success = await _adminService.DeleteUserAsync(email);
            return success
                ? Ok("Đã xóa người dùng thành công!")
                : BadRequest("Xóa thất bại hoặc không tìm thấy người dùng.");
        }

        [HttpGet]
        public IActionResult LoadAddUserForm()
        {
            return PartialView("Partials/_AddUserForm", new UserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join("<br/>", errors));
            }

            var success = await _adminService.AddUserAsync(model);
            return success
                ? Ok("Đã thêm người dùng thành công!")
                : BadRequest("Email đã tồn tại hoặc lỗi khi thêm người dùng.");
        }

        // === BÁO CÁO ===
        public async Task<IActionResult> ReportList()
        {
            var reports = await _adminService.GetAllReportViewModelsAsync();
            return View(reports);
        }

        public async Task<IActionResult> Report(int bookId)
        {
            var report = await _adminService.GetReportByBookIdAsync(bookId);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy báo cáo.";
                return RedirectToAction("ReportList");
            }

            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Report(int bookId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do báo cáo.";
                return RedirectToAction("Index", "Chapter", new { bookId });
            }

            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _adminService.CreateReportAsync(bookId, reason, reporterId);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Báo cáo đã được gửi thành công!" : "Gửi báo cáo thất bại!";

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> HandleReport(int id, string status)
        {
            var success = await _adminService.UpdateReportStatusAsync(id, status);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Đã cập nhật trạng thái báo cáo!" : "Cập nhật thất bại!";
            return RedirectToAction("ReportList");
        }
    }
}
