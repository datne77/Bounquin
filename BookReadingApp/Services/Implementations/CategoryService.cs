using BookReadingApp.Models;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<List<Category>> GetAllCategoriesAsync()
        => _categoryRepository.GetAllAsync();

    public Task<Category?> GetCategoryByIdAsync(int id)
        => _categoryRepository.GetByIdAsync(id);

    public Task AddCategoryAsync(Category category)
        => _categoryRepository.AddAsync(category);

    public Task UpdateCategoryAsync(Category category)
        => _categoryRepository.UpdateAsync(category);

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
        }
    }
}