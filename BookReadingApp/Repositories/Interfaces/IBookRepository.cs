using BookReadingApp.Models;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllWithCategoryAsync();
    Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Book?> GetBookWithChaptersAsync(int id);
    Task<Book?> GetByIdAsync(int id);
    Task CreateAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    Task<List<Book>> SearchByTitleAsync(string keyword);
    Task<IEnumerable<Book>> GetAllAsync();
    Task IncreaseViewCountAsync(int bookId);
    Task<List<Book>> GetMostViewedBooksAsync(int limit = 10);
    Task<List<ClassicBookEntry>> GetClassicBookEntriesAsync();



}
