using BookReadingApp.Data;
using BookReadingApp.Models;
using Microsoft.EntityFrameworkCore;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Review>> GetByBookIdAsync(int bookId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<(double avg, int count)> GetStatsAsync(int bookId)
    {
        var reviews = _context.Reviews.Where(r => r.BookId == bookId);
        int count = await reviews.CountAsync();
        double avg = count > 0 ? await reviews.AverageAsync(r => r.Rating) : 0;
        return (avg, count);
    }

    public async Task<bool> HasUserReviewedAsync(int bookId, string userId)
    {
        return await _context.Reviews.AnyAsync(r => r.BookId == bookId && r.UserId == userId);
    }
}
