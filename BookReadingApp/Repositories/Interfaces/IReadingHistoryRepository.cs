using BookReadingApp.Models;

public interface IReadingHistoryRepository
{
    Task SaveReadingAsync(string userId, int bookId);
    Task<List<Book>> GetRecentlyReadBooksAsync(string userId, int limit = 5);
}
