using BookReadingApp.Models;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<List<Review>> GetByBookIdAsync(int bookId);
    Task<(double avg, int count)> GetStatsAsync(int bookId);
    Task<bool> HasUserReviewedAsync(int bookId, string userId);
}
