using BookReadingApp.Models;

public class ChapterService : IChapterService
{
    private readonly IChapterRepository _repo;

    public ChapterService(IChapterRepository repo)
    {
        _repo = repo;
    }

    public Task<Book?> GetBookWithChaptersAsync(int bookId) => _repo.GetBookWithChaptersAsync(bookId);

    public Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId) => _repo.GetChaptersByBookIdAsync(bookId);

    public Task<Chapter?> GetChapterWithBookAsync(int chapterId) => _repo.GetChapterWithBookAsync(chapterId);

    public Task<List<Comment>> GetCommentsByChapterIdAsync(int chapterId) => _repo.GetCommentsByChapterIdAsync(chapterId);

    public async Task AddChapterFromFileAsync(int bookId, IFormFile uploadedFile)
    {
        using var reader = new StreamReader(uploadedFile.OpenReadStream());
        var content = await reader.ReadToEndAsync();

        var chapter = new Chapter
        {
            BookId = bookId,
            Title = Path.GetFileNameWithoutExtension(uploadedFile.FileName),
            Content = content,
            CreatedDate = DateTime.Now
        };

        await _repo.AddChapterAsync(chapter);
    }

    public Task UpdateChapterAsync(Chapter chapter) => _repo.UpdateChapterAsync(chapter);

    public async Task DeleteChapterAsync(int chapterId)
    {
        var chapter = await _repo.GetChapterWithBookAsync(chapterId);
        if (chapter != null)
        {
            await _repo.DeleteChapterAsync(chapter);
        }
    }

    public Task AddCommentAsync(Comment comment) => _repo.AddCommentAsync(comment);

    public Task<Comment?> GetCommentByIdAsync(int commentId) => _repo.GetCommentByIdAsync(commentId);

    public Task DeleteCommentAsync(Comment comment) => _repo.DeleteCommentAsync(comment);
}
