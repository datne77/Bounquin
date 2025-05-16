using BookReadingApp.Models;
using BookReadingApp.Services.Interfaces;
using BookReadingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService,
            IAccountRepository accountRepo,
            ICategoryRepository categoryRepo,
            IBookRepository bookRepo
        ) : base(accountRepo, categoryRepo, bookRepo)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategoryAsync(category);
                TempData["SuccessMessage"] = "Thể loại mới đã được thêm!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Lỗi khi thêm thể loại!";
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                TempData["SuccessMessage"] = "Thể loại đã được cập nhật!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Lỗi khi cập nhật thể loại!";
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            TempData["SuccessMessage"] = "Thể loại đã được xóa!";
            return RedirectToAction(nameof(Index));
        }
    }
}
