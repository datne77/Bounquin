using BookReadingApp.Data;
using BookReadingApp.Models;
using Microsoft.EntityFrameworkCore;

public class ReadingHistoryRepository : IReadingHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public ReadingHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveReadingAsync(string userId, int bookId)
    {
        var history = await _context.ReadingHistories
            .FirstOrDefaultAsync(h => h.UserId == userId && h.BookId == bookId);

        if (history != null)
        {
            history.LastReadTime = DateTime.Now;
        }
        else
        {
            _context.ReadingHistories.Add(new ReadingHistory
            {
                UserId = userId,
                BookId = bookId,
                LastReadTime = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Book>> GetRecentlyReadBooksAsync(string userId, int limit = 5)
    {
        return await _context.ReadingHistories
         .Where(h => h.UserId == userId)
         .Include(h => h.Book).ThenInclude(b => b.Category)
         .OrderByDescending(h => h.LastReadTime)
         .Select(h => h.Book)
         .Take(limit)
         .ToListAsync();

    }
}
