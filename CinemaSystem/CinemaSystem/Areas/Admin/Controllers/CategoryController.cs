using CinemaSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(CategoryVM catvm)
        {
            var categories = _context.Categories.AsQueryable();

            categories = categories.Skip((catvm.Page - 1) * 4).Take(4);
            ViewBag.CurrentPage = catvm.Page;
            ViewBag.TotalPages = Math.Ceiling(_context.Categories.Count() / 4.0);
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CategoryVM catvm)
        {
            Category cat = new Category()
            {
                Name = catvm.Name,
                Description = catvm.Description,
                Status =  catvm.Status
            };
            _context.Categories.Add(cat);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == Id);

            return View(cat);
        }
        [HttpPost]
        public IActionResult Update(Category cat)
        {
            var c = _context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == cat.Id);
            
            _context.Categories.Update(cat);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == Id);
            
            _context.Categories.Remove(cat);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
