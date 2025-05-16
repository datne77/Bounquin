using BookReadingApp.Models;

public interface IChapterRepository
{
    Task<Book?> GetBookWithChaptersAsync(int bookId);
    Task<Chapter?> GetChapterWithBookAsync(int chapterId);
    Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId);
    Task<List<Comment>> GetCommentsByChapterIdAsync(int chapterId);
    Task AddChapterAsync(Chapter chapter);
    Task UpdateChapterAsync(Chapter chapter);
    Task DeleteChapterAsync(Chapter chapter);
    Task AddCommentAsync(Comment comment);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task DeleteCommentAsync(Comment comment);
}