using BookReadingApp.Data;
using BookReadingApp.Models;
using Microsoft.EntityFrameworkCore;

public class ChapterRepository : IChapterRepository
{
    private readonly ApplicationDbContext _context;

    public ChapterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetBookWithChaptersAsync(int bookId)
        => await _context.Books.Include(b => b.Chapters).FirstOrDefaultAsync(b => b.Id == bookId);

    public async Task<List<Chapter>> GetChaptersByBookIdAsync(int bookId)
        => await _context.Chapters.Where(c => c.BookId == bookId).ToListAsync();

    public async Task<Chapter?> GetChapterWithBookAsync(int chapterId)
        => await _context.Chapters.Include(c => c.Book).ThenInclude(b => b.Chapters).FirstOrDefaultAsync(c => c.Id == chapterId);

    public async Task<List<Comment>> GetCommentsByChapterIdAsync(int chapterId)
    {
        return await _context.Comments
            .Where(c => c.ChapterId == chapterId)
            .OrderByDescending(c => c.CreatedDate)
            .Select(c => new Comment
            {
                Id = c.Id,
                Content = c.Content,
                CreatedDate = c.CreatedDate,
                UserId = c.UserId,
                ChapterId = c.ChapterId,
                User = _context.Users.FirstOrDefault(u => u.Id == c.UserId)
            })
            .ToListAsync();
    }

    public async Task AddChapterAsync(Chapter chapter)
    {
        _context.Chapters.Add(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateChapterAsync(Chapter chapter)
    {
        _context.Chapters.Update(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChapterAsync(Chapter chapter)
    {
        _context.Chapters.Remove(chapter);
        await _context.SaveChangesAsync();
    }

    public async Task AddCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId)
        => await _context.Comments.FindAsync(commentId);

    public async Task DeleteCommentAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}