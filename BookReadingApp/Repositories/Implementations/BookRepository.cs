using BookReadingApp.Data;
using BookReadingApp.Models;
using Microsoft.EntityFrameworkCore;


public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllWithCategoryAsync()
        => await _context.Books.Include(b => b.Category).ToListAsync();

    public async Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId)
        => await _context.Books.Include(b => b.Category).Where(b => b.CategoryId == categoryId).ToListAsync();

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        => await _context.Categories.ToListAsync();

    public async Task<Book?> GetBookWithChaptersAsync(int id)
        => await _context.Books.Include(b => b.Chapters).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Book?> GetByIdAsync(int id)
        => await _context.Books.FindAsync(id);

    public async Task CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Book>> SearchByTitleAsync(string keyword)
    {
        return await _context.Books
            .Where(b => b.Title.Contains(keyword))
            .OrderBy(b => b.Title)
            .Take(10)
            .ToListAsync();
    }
    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Category) // Optional
            .ToListAsync();
    }
    public async Task IncreaseViewCountAsync(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book != null)
        {
            book.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Book>> GetMostViewedBooksAsync(int limit = 10)
    {
        return await _context.Books
            .OrderByDescending(b => b.ViewCount)
            .Take(limit)
            .ToListAsync();
    }
    public async Task<List<ClassicBookEntry>> GetClassicBookEntriesAsync()
    {
        return await _context.ClassicBookEntries
            .Include(e => e.Book)
            .ThenInclude(b => b.Category)
            .OrderBy(e => e.DisplayOrder)
            .ToListAsync();
    }

}
