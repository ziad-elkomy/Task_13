using CinemaSystem.Data;
using CinemaSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Actor> ActorRepository; //= new Repository<Actor>();

        public ActorController(IRepository<Actor> actorRepository)
        {
            ActorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(ActorVM actvm)
        {
            var actors = await ActorRepository.GetAllAsync();//Modify

            actors = actors.Skip((actvm.Page - 1) * 4).Take(4);
            ViewBag.CurrentPage = actvm.Page;
            ViewBag.TotalPages = Math.Ceiling(ActorRepository.GetCount() / 4.0);//Modify
            return View(actors.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ActorVM actvm)
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

            await ActorRepository.AddAsync(act);//Modify
            await ActorRepository.SaveChangesAsync();//Modify
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var acts = await ActorRepository.GetFirstAsync(c => c.Id == Id);//Modify

            return View(acts);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Actor actor, IFormFile ImageFile)
        {
            //var c = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);
            var c = await ActorRepository.GetFirstAsync(c => c.Id == actor.Id, isTracked: false);
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
            //_context.Actors.Update(actor);
            //_context.SaveChanges();
            ActorRepository.Update(actor);
            await ActorRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            //var act = _context.Actors.FirstOrDefault(c => c.Id == Id);
            var act = await ActorRepository.GetFirstAsync(c => c.Id == Id);
            if (act.Image != null)
            {
                string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Cinema\\", act.Image);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
            }
            //_context.Actors.Remove(act);
            //_context.SaveChanges();
            ActorRepository.Remove(act);
            await ActorRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
