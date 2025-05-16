using BookReadingApp.Models;
using BookReadingApp.Repositories.Interfaces;
using BookReadingApp.Services.Interfaces;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;

    public ReviewService(IReviewRepository reviewRepo)
    {
        _reviewRepo = reviewRepo;
    }

    public Task AddReviewAsync(Review review) => _reviewRepo.AddAsync(review);

    public Task<List<Review>> GetReviewsByBookIdAsync(int bookId) => _reviewRepo.GetByBookIdAsync(bookId);

    public Task<(double avg, int count)> GetReviewStatsAsync(int bookId) => _reviewRepo.GetStatsAsync(bookId);

    public Task<bool> HasUserReviewedAsync(int bookId, string userId) => _reviewRepo.HasUserReviewedAsync(bookId, userId);
}
