using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.Employee_ROLE}")]
    public class CategoryController : Controller
    {
        IRepository<Category> _categoryRepository;
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAllAsync().Result;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create(Category? category)
        {
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category, int? x = 0)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize($"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }
    }
}
