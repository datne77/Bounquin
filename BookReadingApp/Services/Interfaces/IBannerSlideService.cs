using BookReadingApp.Models;

namespace BookReadingApp.Services.Interfaces
{
    public interface IBannerSlideService
    {
        Task<List<BannerSlide>> GetAllAsync();
        Task<BannerSlide?> GetByIdAsync(int id);
        Task AddAsync(BannerSlide banner);
        Task UpdateAsync(BannerSlide banner);
        Task DeleteAsync(int id);
        Task<List<BannerSlide>> GetActiveSlidesAsync();
    }
}
