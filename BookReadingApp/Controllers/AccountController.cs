using BookReadingApp.Services.Interfaces;
using BookReadingApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookReadingApp.Repositories.Interfaces;

namespace BookReadingApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(
            IAccountService accountService,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo,
            IBookRepository bookRepo
        ) : base(accountRepo, categoryRepo, bookRepo)
        {
            _accountService = accountService;
        }


        // GET: Account/Register
        public IActionResult Register() => View();

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!";
                return View(model);
            }

            var success = await _accountService.RegisterAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Index", "Book");
            }

            TempData["ErrorMessage"] = "Đăng ký thất bại. Vui lòng thử lại.";
            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login() => View();

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                return View(model);
            }

            var success = await _accountService.LoginAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Book");
            }

            TempData["ErrorMessage"] = "Đăng nhập không hợp lệ. Vui lòng kiểm tra lại email và mật khẩu.";
            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _accountService.GetProfileAsync(userId!);

            if (profile == null) return RedirectToAction("Login");
            ViewBag.CurrentUserId = userId;

            return View(profile);
        }

        // GET: Account/Details
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _accountService.GetDetailsAsync(userId!);

            if (model == null) return RedirectToAction("Login");
            return View(model);
        }

        // GET: Account/ProfileEdit
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProfileEdit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _accountService.GetDetailsAsync(userId!);

            if (model == null) return RedirectToAction("Login");

            return View(model);
        }

        // POST: Account/ProfileEdit
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }

            var success = await _accountService.UpdateProfileAsync(model, model.ProfilePicture);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = "Cập nhật thất bại!";
            return View(model);
        }
    }
}
                    