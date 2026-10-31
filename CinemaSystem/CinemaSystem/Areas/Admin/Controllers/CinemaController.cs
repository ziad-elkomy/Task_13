using CinemaSystem.Data;
using CinemaSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Cinema> CinemaRepository; //= new Repository<Cinema>();

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            CinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(CinemaVM cinema)
        {

            //var cinemas = _context.Cinemas.AsQueryable();
            var cinemas = await CinemaRepository.GetAllAsync();
            
            cinemas = cinemas.Skip((cinema.Page-1)*4).Take(4);
            ViewBag.CurrentPage = cinema.Page;
            //ViewBag.TotalPages = Math.Ceiling(_context.Cinemas.Count()/4.0);
            ViewBag.TotalPages = Math.Ceiling(CinemaRepository.GetCount() / 4.0);
            return View(cinemas.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CinemaVM cinema)
        {
            Cinema c = new Cinema()
            {
                Name = cinema.Name,
                Description = cinema.Description,
                Status = cinema.Status
            };
            if (cinema.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + cinema.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\" + fileName);
                using(var stream = System.IO.File.Create(filePath))
                {
                    cinema.ImageFile.CopyTo(stream);
                }
                c.Image = fileName;
            }

            //_context.Cinemas.Add(c);
            //_context.SaveChanges();
            await CinemaRepository.AddAsync(c);
            await CinemaRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            //var c = _context.Cinemas.FirstOrDefault(c => c.Id == Id);
            var c = await CinemaRepository.GetFirstAsync(c => c.Id == Id);
            
            return View(c);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema , IFormFile ImageFile)
        {
            //var c = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);
            var c = await CinemaRepository.GetFirstAsync(c => c.Id == cinema.Id , isTracked: false);
            string oldfilename = c.Image;
            if(ImageFile!=null)
            {
                string filename = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\", filename);
                using(var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                cinema.Image = filename;
                string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\" , oldfilename);
                if(System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            else
            {
                cinema.Image = oldfilename;
            }
            //_context.Cinemas.Update(cinema);
            //_context.SaveChanges();
            CinemaRepository.Update(cinema);
            await CinemaRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            //var c = _context.Cinemas.FirstOrDefault(c => c.Id == Id);
            var c = await CinemaRepository.GetFirstAsync(c => c.Id == Id);
            
            if(c.Image != null)
            {
                string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\", c.Image);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            //_context.Cinemas.Remove(c);
            //_context.SaveChanges();
            CinemaRepository.Remove(c);
            await CinemaRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
