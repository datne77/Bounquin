using BookReadingApp.Models;

public interface IChapterService
{
    Task<Book?> GetBookWithChaptersAsync(int bookId);
    Task<Chapter?> GetChapterWithBookAsync(int chapterId);
    Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId);
    Task<List<Comment>> GetCommentsByChapterIdAsync(int chapterId);
    Task AddChapterFromFileAsync(int bookId, IFormFile uploadedFile);
    Task UpdateChapterAsync(Chapter updated);
    Task DeleteChapterAsync(int chapterId);
    Task AddCommentAsync(Comment comment);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task DeleteCommentAsync(Comment comment);
}
