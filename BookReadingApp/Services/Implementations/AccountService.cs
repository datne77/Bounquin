using BookReadingApp.Data;
using BookReadingApp.Models;
using BookReadingApp.Repositories.Interfaces;
using BookReadingApp.Services.Interfaces;
using BookReadingApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BookReadingApp.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ApplicationDbContext _context;

        public AccountService(IAccountRepository accountRepo, ApplicationDbContext context)
        {
            _accountRepo = accountRepo;
            _context = context;
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = "Chưa cập nhật",
                Bio = "",
                ProfilePictureUrl = "/uploads/default-avatar.png"
            };
            var result = await _accountRepo.RegisterUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var result = await _accountRepo.SignInAsync(model.Email, model.Password, model.RememberMe);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _accountRepo.SignOutAsync();
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _accountRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            var books = await _context.Books
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Books = books
            };
        }

        public async Task<ProfileViewModel?> GetDetailsAsync(string userId)
        {
            var user = await _accountRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<bool> UpdateProfileAsync(ProfileViewModel model, IFormFile? profilePicture)
        {
            var user = await _accountRepo.GetUserByIdAsync(model.Id!);
            if (user == null) return false;

            // Giữ thông tin cũ nếu input rỗng
            user.FullName = string.IsNullOrWhiteSpace(model.FullName) ? user.FullName : model.FullName;
            user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            user.Bio = string.IsNullOrWhiteSpace(model.Bio) ? user.Bio : model.Bio;

            // ✅ Xử lý ảnh đại diện
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return false; // từ chối file không hợp lệ

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                // Xoá ảnh cũ nếu không phải mặc định
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.Contains("default-avatar"))
                {
                    var oldPath = Path.Combine(uploadFolder, Path.GetFileName(user.ProfilePictureUrl));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // Tạo tên file mới duy nhất: userId_timestamp.ext
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var newFileName = $"{user.Id}_{timestamp}{extension}";
                var newFilePath = Path.Combine(uploadFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = "/uploads/" + newFileName;
            }

            var result = await _accountRepo.UpdateUserAsync(user);
            return result.Succeeded;
        }


    }
}
