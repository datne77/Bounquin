using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookReadingApp.Models;
using BookReadingApp.Services.Interfaces;
using BookReadingApp.Repositories.Interfaces;

namespace BookReadingApp.Controllers
{
    [Authorize]
    public class ChapterController : BaseController
    {
        private readonly IChapterService _chapterService;
        private readonly IReadingHistoryRepository _readingHistoryRepo;


        public ChapterController(
            IChapterService chapterService,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo, 
            IBookRepository bookRepo,
            IReadingHistoryRepository readingHistoryRepo
        ) : base(accountRepo, categoryRepo, bookRepo)
        {
            _chapterService = chapterService;
            _readingHistoryRepo = readingHistoryRepo;
        }

        public async Task<IActionResult> Index(int bookId)
        {
            var book = await _chapterService.GetBookWithChaptersAsync(bookId);
            if (book == null) return NotFound();

            Response.Cookies.Append($"LastViewedBookTime_{bookId}", DateTime.Now.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true
            });

            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            ViewBag.Book = book;
            ViewBag.BookId = bookId;
            ViewBag.BookOwnerId = book.UserId;
            ViewBag.CurrentUserId = userId;
            ViewBag.IsOwner = book.UserId == userId;
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View(book.Chapters);
        }

        public async Task<IActionResult> Read(int chapterId, bool showLastViewedTime = false)
        {
            var chapter = await _chapterService.GetChapterWithBookAsync(chapterId);
            if (chapter == null || chapter.Book == null) return NotFound();
            await _bookRepo.IncreaseViewCountAsync(chapter.BookId);

            // ✅ Ghi nhận lịch sử đọc
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await _readingHistoryRepo.SaveReadingAsync(userId, chapter.BookId);
            }

            var chapters = chapter.Book.Chapters.OrderBy(c => c.Id).ToList();
            int index = chapters.FindIndex(c => c.Id == chapterId);

            string cookieKey = $"LastViewedBookTime_{chapterId}";
            if (!showLastViewedTime)
            {
                Response.Cookies.Append(cookieKey, DateTime.Now.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true
                });
            }

            ViewBag.LastViewedTime = HttpContext.Request.Cookies[cookieKey];
            ViewBag.PreviousChapter = index > 0 ? chapters[index - 1] : null;
            ViewBag.NextChapter = index < chapters.Count - 1 ? chapters[index + 1] : null;
            ViewBag.Book = chapter.Book;

            var comments = await _chapterService.GetCommentsByChapterIdAsync(chapterId);
            ViewBag.Comments = comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedDate,
                c.UserId,
                UserFullName = c.User?.FullName ?? "",
                ProfilePictureUrl = c.User?.ProfilePictureUrl ?? ""
            }).ToList();

            return View(chapter);
        }

        [HttpGet]
        public IActionResult Create(int bookId)
        {
            ViewBag.BookId = bookId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId, IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file hợp lệ.";
                return RedirectToAction("Create", new { bookId });
            }

            await _chapterService.AddChapterFromFileAsync(bookId, uploadedFile);
            TempData["SuccessMessage"] = "Chương đã được thêm thành công!";
            return RedirectToAction("Index", new { bookId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var chapter = await _chapterService.GetChapterWithBookAsync(id);
            if (chapter == null || chapter.Book == null) return RedirectToAction("Index", "Book");

            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (chapter.Book.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            return View(chapter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Content, BookId")] Chapter chapter)
        {
            if (id != chapter.Id) return NotFound();

            var existing = await _chapterService.GetChapterWithBookAsync(id);
            if (existing == null || existing.Book == null) return RedirectToAction("Index", "Book");

            if (existing.Book.UserId != User?.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
                return Forbid();

            existing.Title = chapter.Title;
            existing.Content = chapter.Content;

            await _chapterService.UpdateChapterAsync(existing);
            TempData["SuccessMessage"] = "Chương đã được cập nhật!";
            return RedirectToAction("Index", new { bookId = existing.BookId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var chapter = await _chapterService.GetChapterWithBookAsync(id);
            if (chapter == null || chapter.Book == null)
                return NotFound("Không tìm thấy chương hoặc sách!");

            if (chapter.Book.UserId != User?.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
                return Forbid();

            await _chapterService.DeleteChapterAsync(id);
            return Ok("Chương đã được xóa.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int bookId, int chapterId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Nội dung bình luận không được để trống!";
                return RedirectToAction("Read", new { bookId, chapterId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để bình luận!";
                return RedirectToAction("Read", new { bookId, chapterId });
            }

            var comment = new Comment
            {
                UserId = userId,
                ChapterId = chapterId,
                Content = content,
                CreatedDate = DateTime.Now
            };

            await _chapterService.AddCommentAsync(comment);
            TempData["SuccessMessage"] = "Bình luận đã được thêm!";
            return RedirectToAction("Read", new { bookId, chapterId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int chapterId)
        {
            var comment = await _chapterService.GetCommentByIdAsync(commentId);
            if (comment == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xóa bình luận này!";
                return RedirectToAction("Read", new { chapterId });
            }

            await _chapterService.DeleteCommentAsync(comment);
            return RedirectToAction("Read", new { chapterId });
        }
    }
}
