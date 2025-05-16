using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReadingApp.Models;
using BookReadingApp.Models.ViewModels;
using BookReadingApp.Services.Interfaces;
using BookReadingApp.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BookReadingApp.Data;

namespace BookReadingApp.Controllers
{
    [Authorize]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IBannerSlideService _bannerSlideService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public BookController(
            IBookService bookService,
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo,
            IBookRepository bookRepo,
            IBannerSlideService bannerSlideService
        ) : base(accountRepo, categoryRepo, bookRepo)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _userManager = userManager;
            _env = env;
            _bannerSlideService = bannerSlideService;
            
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksGroupedByCategoryAsync();
            var categories = await _bookService.GetAllCategoriesAsync();
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            ViewBag.ClassicBooks = await _bookService.GetClassicBookEntriesAsync();

            // ✅ Thêm dòng này để truyền sách đề xuất
            ViewBag.RecommendedBooks = await _bookService.GetRecommendedBooksAsync();
            ViewBag.ReadingBooks = await _bookService.GetReadingBooksAsync(userId);

            ViewBag.Categories = categories ?? new List<Category>();
            var booksByCategory = categories.ToDictionary(
                category => category.Id,
                category => books.Where(b => b.CategoryId == category.Id).ToList()
            );

            ViewBag.BooksByCategory = booksByCategory;
            ViewBag.SelectedCategory = "Tất cả sách";
            ViewBag.CurrentUserId = userId;

            ViewBag.SlideBanners = await _bannerSlideService.GetActiveSlidesAsync();

            return View(books);
        }


        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookDetailsAsync(id);
            if (book == null) return NotFound();

            var (avgStars, totalReviews) = await _reviewService.GetReviewStatsAsync(id);
            ViewBag.AverageStars = avgStars;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.Reviews = await _reviewService.GetReviewsByBookIdAsync(id);

            return RedirectToAction("Index", "Chapter", new { bookId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Đánh giá không hợp lệ!";
                return RedirectToAction("Details", new { id = model.BookId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (await _reviewService.HasUserReviewedAsync(model.BookId, userId))
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sách này rồi!";
                return RedirectToAction("Details", new { id = model.BookId });
            }

            var review = new Review
            {
                BookId = model.BookId,
                Rating = model.Rating,
                Comment = model.Comment,
                UserId = userId,
                CreatedDate = DateTime.Now
            };

            await _reviewService.AddReviewAsync(review);
            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá sách!";
            return RedirectToAction("Details", new { id = model.BookId });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return RedirectToAction("Index");
            }

            var books = await _bookService.SearchBooksAsync(query);
            ViewBag.Categories = await _bookService.GetAllCategoriesAsync();
            ViewBag.BooksByCategory = new Dictionary<int, List<Book>> { { 0, books.ToList() } };
            ViewBag.SelectedCategory = $"Kết quả tìm kiếm cho '{query}'";

            return View("Index", books);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(int? categoryId)
        {
            if (categoryId == null || categoryId == 0)
            {
                TempData["ErrorMessage"] = "Thể loại không hợp lệ.";
                return RedirectToAction("Index");
            }

            var categories = await _bookService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            var books = await _bookService.GetBooksByCategoryAsync(categoryId.Value);
            ViewBag.BooksByCategory = new Dictionary<int, List<Book>> { { categoryId.Value, books.ToList() } };
            ViewBag.SelectedCategory = categories.FirstOrDefault(c => c.Id == categoryId)?.Name ?? "Không xác định";
            ViewBag.SelectedCategoryId = categoryId;

            return View("Index", books);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _bookService.GetAllCategoriesAsync();
            if (!categories.Any())
            {
                TempData["ErrorMessage"] = "Không có thể loại nào. Vui lòng thêm thể loại trước!";
                return RedirectToAction("Create", "Category");
            }

            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Author,CategoryId")] Book book, IFormFile? coverImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thêm sách.";
                return RedirectToAction("Login", "Account");
            }

            book.UserId = user.Id;
            book.CreatedDate = DateTime.Now;

            if (book.CategoryId == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn thể loại!";
                ViewBag.Categories = await _bookService.GetAllCategoriesAsync();
                return View(book);
            }

            await _bookService.CreateBookAsync(book, coverImage, _env.WebRootPath);
            TempData["SuccessMessage"] = "Sách đã được thêm thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _bookService.GetBookDetailsAsync(id.Value);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (book.UserId != user?.Id && !User.IsInRole("Admin")) return Forbid();

            ViewBag.Categories = await _bookService.GetAllCategoriesAsync();
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Author,CategoryId")] Book book, IFormFile? coverImage)
        {
            if (id != book.Id) return NotFound();

            var existingBook = await _bookService.GetBookDetailsAsync(id);
            if (existingBook == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (existingBook.UserId != user?.Id && !User.IsInRole("Admin")) return Forbid();

            book.UserId = existingBook.UserId;
            book.CreatedDate = existingBook.CreatedDate;
            await _bookService.UpdateBookAsync(book, coverImage, _env.WebRootPath);
            TempData["SuccessMessage"] = "Sách đã được cập nhật!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _bookService.GetBookDetailsAsync(id.Value);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && book.UserId != user.Id) return Forbid();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookService.GetBookDetailsAsync(id);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && book.UserId != user.Id) return Forbid();

            await _bookService.DeleteBookAsync(book);
            TempData["SuccessMessage"] = "Sách đã được xóa!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new List<object>());

            var books = await _bookService.SearchBooksByTitleAsync(term);
            var results = books.Select(b => new
            {
                id = b.Id,
                title = b.Title,
                cover = string.IsNullOrEmpty(b.CoverImageUrl) ? "/images/default-book-cover.jpg" : b.CoverImageUrl
            }).ToList();

            return Json(results);
        }
    }
}
