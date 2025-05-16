using BookReadingApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReadingApp.Models;

[Authorize(Roles = "Admin")]
public class ClassicBookController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IBookService _bookService;

    public ClassicBookController(ApplicationDbContext context, IBookService bookService)
    {
        _context = context;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var classics = await _context.ClassicBookEntries
                            .Include(e => e.Book).ThenInclude(b => b.Category)
                            .OrderBy(e => e.DisplayOrder)
                            .ToListAsync();
       return View(classics);
    }

    public async Task<IActionResult> Create()
    {
        var books = await _bookService.GetAllBooksGroupedByCategoryAsync();

        var grouped = books
            .GroupBy(b => b.Category?.Name ?? "Khác")
            .Select(g => new BookDropdownGroupViewModel
            {
                CategoryName = g.Key,
                Books = g.ToList()
            }).ToList();

        ViewBag.GroupedBooks = grouped;

        return View(new ClassicBookEntry());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassicBookEntry entry)
    {
        if (entry.BookId == 0)
        {
            TempData["Error"] = "Vui lòng chọn sách hợp lệ.";
            return RedirectToAction(nameof(Create));
        }

        _context.ClassicBookEntries.Add(entry);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm sách kinh điển thành công!";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadGroupedBooks()
    {
        var books = await _bookService.GetAllBooksGroupedByCategoryAsync();
        var grouped = books
            .GroupBy(b => b.Category?.Name ?? "Khác")
            .Select(g => new BookDropdownGroupViewModel
            {
                CategoryName = g.Key,
                Books = g.ToList()
            }).ToList();
        ViewBag.GroupedBooks = grouped;
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _context.ClassicBookEntries.FindAsync(id);
        if (entry != null)
        {
            _context.ClassicBookEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
