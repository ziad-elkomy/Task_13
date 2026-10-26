using CinemaSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(ActorVM actvm)
        {
            var actors = _context.Actors.AsQueryable();

            actors = actors.Skip((actvm.Page - 1) * 4).Take(4);
            ViewBag.CurrentPage = actvm.Page;
            ViewBag.TotalPages = Math.Ceiling(_context.Actors.Count() / 4.0);
            return View(actors.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ActorVM actvm)
        {
            Actor act = new Actor()
            {
                Name = actvm.Name,
                Description = actvm.Description
            };
            if (actvm.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + actvm.ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Actor\\" + fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    actvm.ImageFile.CopyTo(stream);
                }
                act.Image = fileName;
            }

            _context.Actors.Add(act);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var acts = _context.Actors.FirstOrDefault(c => c.Id == Id);

            return View(acts);
        }
        [HttpPost]
        public IActionResult Update(Actor actor, IFormFile ImageFile)
        {
            var c = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);
            string oldfilename = c.Image;
            if (ImageFile != null)
            {
                string filename = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Actor\\", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                actor.Image = filename;
                string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Actor\\", oldfilename);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            else
            {
                actor.Image = oldfilename;
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var act = _context.Actors.FirstOrDefault(c => c.Id == Id);
            if (act.Image != null)
            {
                string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\", act.Image);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            _context.Actors.Remove(act);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
