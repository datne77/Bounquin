using BookReadingApp.Models;
using BookReadingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookReadingApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class BannerSlideController : Controller
    {
        private readonly IBannerSlideService _bannerService;
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _env;

        public BannerSlideController(IBannerSlideService bannerService, IBookService bookService, IWebHostEnvironment env)
        {
            _bannerService = bannerService;
            _bookService = bookService;
            _env = env;
        }

        // Danh sách banner
        public async Task<IActionResult> Index()
        {
            var banners = await _bannerService.GetAllAsync();
            return View(banners);
        }

        // GET: Thêm mới banner
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = new SelectList(await _bookService.GetAllAsync(), "Id", "Title");
            return View();
        }

        // POST: Thêm mới banner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerSlide model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "❌ ModelState không hợp lệ!";
                ViewBag.Books = new SelectList(await _bookService.GetAllAsync(), "Id", "Title");
                return View(model);
            }

            // Kiểm tra file ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "banners");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var path = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                model.ImageUrl = "/uploads/banners/" + fileName;
            }

            await _bannerService.AddAsync(model);
            TempData["SuccessMessage"] = "✅ Banner đã được thêm!";
            return RedirectToAction("Index");
        }


        // GET: Sửa banner
        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner == null) return NotFound();

            ViewBag.Books = new SelectList(await _bookService.GetAllAsync(), "Id", "Title", banner.LinkToBookId);
            return View(banner);
        }

        // POST: Sửa banner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BannerSlide model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Books = new SelectList(await _bookService.GetAllAsync(), "Id", "Title", model.LinkToBookId);
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "banners");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var path = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                model.ImageUrl = "/uploads/banners/" + fileName;
            }

            await _bannerService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        // POST: Xóa banner
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _bannerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
