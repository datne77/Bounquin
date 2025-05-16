using BookReadingApp.Models;

public interface IReviewService
{
    Task AddReviewAsync(Review review);
    Task<List<Review>> GetReviewsByBookIdAsync(int bookId);
    Task<(double avg, int count)> GetReviewStatsAsync(int bookId);
    Task<bool> HasUserReviewedAsync(int bookId, string userId);
}
