using BookReadingApp.Models;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksGroupedByCategoryAsync()
        => await _bookRepository.GetAllWithCategoryAsync();

    public async Task<IEnumerable<Book>> SearchBooksAsync(string query)
    {
        var books = await _bookRepository.GetAllWithCategoryAsync();
        return books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        => await _bookRepository.GetAllCategoriesAsync();

    public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        => await _bookRepository.GetByCategoryAsync(categoryId);

    public async Task<Book?> GetBookDetailsAsync(int id)
        => await _bookRepository.GetBookWithChaptersAsync(id);

    public async Task CreateBookAsync(Book book, IFormFile? coverImage, string rootPath)
    {
        if (coverImage != null && coverImage.Length > 0)
        {
            string uploadFolder = Path.Combine(rootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);
            string fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await coverImage.CopyToAsync(stream);
            book.CoverImageUrl = "/uploads/" + fileName;
        }
        else
        {
            book.CoverImageUrl = "/images/default-book-cover.jpg";
        }

        await _bookRepository.CreateAsync(book);
    }

    public async Task UpdateBookAsync(Book book, IFormFile? coverImage, string rootPath)
    {
        if (coverImage != null && coverImage.Length > 0)
        {
            string uploadFolder = Path.Combine(rootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);
            string fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await coverImage.CopyToAsync(stream);
            book.CoverImageUrl = "/uploads/" + fileName;
        }

        await _bookRepository.UpdateAsync(book);
    }
    private readonly IReadingHistoryRepository _readingHistoryRepository;

    public BookService(IBookRepository bookRepository, IReadingHistoryRepository readingHistoryRepository)
    {
        _bookRepository = bookRepository;
        _readingHistoryRepository = readingHistoryRepository;
    }
    public async Task DeleteBookAsync(Book book)
        => await _bookRepository.DeleteAsync(book);
    public Task<List<Book>> SearchBooksByTitleAsync(string keyword) =>
    _bookRepository.SearchByTitleAsync(keyword);
    public async Task<IEnumerable<Book>> GetAllAsync()
    => await _bookRepository.GetAllAsync();
    public async Task<List<Book>> GetReadingBooksAsync(string userId)
    {
        return await _readingHistoryRepository.GetRecentlyReadBooksAsync(userId, 5);
    }
    public Task<List<Book>> GetRecommendedBooksAsync()
    => _bookRepository.GetMostViewedBooksAsync(10);
    public Task<List<ClassicBookEntry>> GetClassicBookEntriesAsync()
    {
        return _bookRepository.GetClassicBookEntriesAsync();
    }

}
