using BookReadingApp.Models;
using BookReadingApp.Repositories.Interfaces;
using BookReadingApp.Services.Implementations;
using BookReadingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BookReadingApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepo;
        private readonly IBannerSlideService _bannerService;

        public HomeController(
            ILogger<HomeController> logger,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo,
            IBookRepository bookRepo,
            IBannerSlideService bannerService
        ) : base(accountRepo, categoryRepo,bookRepo )
        {
            _logger = logger;
            _bookRepo = bookRepo;
            _bannerService = bannerService;
        }

        public async Task<IActionResult> Index()
        {
            // ✅ Lấy banner slideshow
            ViewBag.SlideBanners = await _bannerService.GetActiveSlidesAsync();

            // ✅ Lấy danh sách thể loại
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = categories;

            // ✅ Truyện đề xuất: những truyện có tiêu đề chứa "hot" hoặc "gợi ý"
            var allBooks = (await _bookRepo.GetAllAsync()).ToList();
            ViewBag.RecommendedBooks = allBooks
                .Where(b => b.Title.Contains("hot", StringComparison.OrdinalIgnoreCase)
                         || b.Title.Contains("gợi ý", StringComparison.OrdinalIgnoreCase))
                .Take(10).ToList();

            // ✅ Truyện hoàn thành: có từ khóa "hoàn thành" trong mô tả
            ViewBag.CompletedBooks = allBooks
                .Where(b => b.Description?.Contains("hoàn thành", StringComparison.OrdinalIgnoreCase) == true)
                .Take(10).ToList();

            // ✅ Truyện đang đọc (nếu đã đăng nhập)
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.ReadingBooks = allBooks
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedDate)
                    .Take(5).ToList();
            }
            else
            {
                ViewBag.ReadingBooks = new List<Book>();
            }

            // ✅ Danh sách truyện theo thể loại
            ViewBag.BooksByCategory = categories.ToDictionary(
                category => category.Id,
                category => allBooks.Where(b => b.CategoryId == category.Id).Take(10).ToList()
            );

            return View();
        }

        public IActionResult Privacy() => View();

        public IActionResult About() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
