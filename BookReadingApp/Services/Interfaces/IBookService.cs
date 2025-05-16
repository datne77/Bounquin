using BookReadingApp.Models;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksGroupedByCategoryAsync();
    Task<IEnumerable<Book>> SearchBooksAsync(string query);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
    Task<Book?> GetBookDetailsAsync(int id);
    Task CreateBookAsync(Book book, IFormFile? coverImage, string rootPath);
    Task UpdateBookAsync(Book book, IFormFile? coverImage, string rootPath);
    Task DeleteBookAsync(Book book);
    Task<List<Book>> SearchBooksByTitleAsync(string keyword);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<List<Book>> GetReadingBooksAsync(string userId);
    Task<List<Book>> GetRecommendedBooksAsync();
    Task<List<ClassicBookEntry>> GetClassicBookEntriesAsync();



}
