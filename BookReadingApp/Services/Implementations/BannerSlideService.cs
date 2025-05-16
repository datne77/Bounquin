using BookReadingApp.Data;
using BookReadingApp.Models;
using BookReadingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookReadingApp.Services.Implementations
{
    public class BannerSlideService : IBannerSlideService
    {
        private readonly ApplicationDbContext _context;

        public BannerSlideService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BannerSlide>> GetAllAsync()
        {
            var banners = await _context.BannerSlides
                .Include(b => b.Book)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();

            foreach (var banner in banners)
            {
                if (banner.LinkToBookId.HasValue && banner.Book == null)
                {
                    banner.Book = await _context.Books.FindAsync(banner.LinkToBookId.Value);
                }
            }

            return banners;
        }


        public async Task<BannerSlide?> GetByIdAsync(int id)
        {
            return await _context.BannerSlides.Include(b => b.Book).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(BannerSlide banner)
        {
            _context.BannerSlides.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BannerSlide banner)
        {
            _context.BannerSlides.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.BannerSlides.FindAsync(id);
            if (banner != null)
            {
                _context.BannerSlides.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BannerSlide>> GetActiveSlidesAsync()
        {
            return await _context.BannerSlides
                .Where(b => b.IsActive)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();
        }
    }
}
