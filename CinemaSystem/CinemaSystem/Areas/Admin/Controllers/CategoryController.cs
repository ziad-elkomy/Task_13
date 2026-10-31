using CinemaSystem.Data;
using CinemaSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Category> CategoryRepository; //= new Repository<Category>();

        public CategoryController(IRepository<Category> categoryRepository)
        {
            CategoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(CategoryVM catvm)
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await CategoryRepository.GetAllAsync();

            categories = categories.Skip((catvm.Page - 1) * 4).Take(4);
            ViewBag.CurrentPage = catvm.Page;
            //ViewBag.TotalPages = Math.Ceiling(_context.Categories.Count() / 4.0);
            ViewBag.TotalPages = Math.Ceiling(CategoryRepository.GetCount() / 4.0);
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM catvm)
        {
            Category cat = new Category()
            {
                Name = catvm.Name,
                Description = catvm.Description,
                Status =  catvm.Status
            };
            //_context.Categories.Add(cat);
            //_context.SaveChanges();
            await CategoryRepository.AddAsync(cat);
            await CategoryRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            //var cat = _context.Categories.FirstOrDefault(c => c.Id == Id);
            var cat = await CategoryRepository.GetFirstAsync(c => c.Id == Id);

            return View(cat);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category cat)
        {
            //var c = _context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == cat.Id);
            var c = await CategoryRepository.GetFirstAsync(c => c.Id == cat.Id, isTracked: false);

            //_context.Categories.Update(cat);
            //_context.SaveChanges();
            CategoryRepository.Update(cat);
            await CategoryRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            //var cat = _context.Categories.FirstOrDefault(c => c.Id == Id);
            var cat = await CategoryRepository.GetFirstAsync(c => c.Id == Id);

            //_context.Categories.Remove(cat);
            //_context.SaveChanges();
            CategoryRepository.Remove(cat);
            await CategoryRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
