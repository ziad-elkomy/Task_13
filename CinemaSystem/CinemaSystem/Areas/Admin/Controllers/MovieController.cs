using CinemaSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(FilterMovieVM filtermovie)
        {
            var movies = _context.Movies.AsQueryable();
            if(filtermovie.Price > 0)
            {
                movies = movies.Where(m => m.Price < filtermovie.Price);
            }
            if(filtermovie.CategoryId > 0)
            {
                movies = movies.Where(m => m.CategoryId == filtermovie.CategoryId);
            }
            if(filtermovie.CinemaId > 0)
            {
                movies = movies.Where(m => m.CinemaId == filtermovie.CinemaId);
            }
            if (filtermovie.Status != null)
            {
                movies = movies.Where(m => m.Status == filtermovie.Status);
            }
            if (filtermovie.DT != new DateTime(new DateOnly(1,1,1) , new TimeOnly(0,0)))
            {
                movies = movies.Where(m => m.DateTime < filtermovie.DT);
            }
            movies = movies.Include(m => m.Category).Include(m => m.Cinema);
            movies = movies.Skip((filtermovie.Page - 1) * 4).Take(4);
            ViewBag.CurrentPage = filtermovie.Page;
            ViewBag.TotalPages = Math.Ceiling(movies.Count() / 4.0);
            return View(new MovieIndexVM()
            {
                Movies = movies.ToList(),
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                filter = filtermovie
            });
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View(new MovieIndexVM()
            {
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList()
            });
        }
        [HttpPost]
        public IActionResult Create(MovieVM movievm)
        {
            Movie movie = new Movie()
            {
                Name = movievm.Name,
                Description = movievm.Description,
                CinemaId = movievm.CinemaId,
                CategoryId = movievm.CategoryId,
                Price = movievm.Price,
                DateTime = movievm.DateTime,
                Status = movievm.Status,
            };
            if (movievm.MainImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + movievm.MainImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    movievm.MainImageFile.CopyTo(stream);
                }
                movie.MainImg = fileName;
            }
            var AddedMovie = _context.Movies.Add(movie);
            _context.SaveChanges();
            foreach(var item in movievm.SubImageFiles)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + item.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    item.CopyTo(stream);
                }
                _context.MovieSubImages.Add(new MovieSubImage()
                {
                    SubImage = fileName,
                    MovieId = AddedMovie.Entity.Id
                });
            }
            _context.SaveChanges();
            foreach (var item in movievm.ActorsId)
            {
                _context.MovieActors.Add(new MovieActor()
                {
                    MovieId = AddedMovie.Entity.Id,
                    ActorId = item
                });
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == Id);


            return View(movie);
        }
        [HttpPost]
        public IActionResult Update(MovieVM movievm)
        {
            Movie movie = new Movie()
            {
                Id = movievm.Id,
                Name = movievm.Name,
                Description = movievm.Description,
                CinemaId = movievm.CinemaId,
                CategoryId = movievm.CategoryId,
                Price = movievm.Price,
                DateTime = movievm.DateTime,
                Status = movievm.Status,
            };
            var MovieInDb = _context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == movievm.Id);
            var subimages = _context.MovieSubImages.Where(c => c.MovieId == movievm.Id);
            var actorsmovie = _context.MovieActors.AsNoTracking().Where(c => c.MovieId == c.MovieId);
            if (movievm.MainImageFile != null)
            {
                var oldfilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , MovieInDb.MainImg);
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
                var fileName = Guid.NewGuid().ToString() + "-" + movievm.MainImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    movievm.MainImageFile.CopyTo(stream);
                }
                movie.MainImg = fileName;
            }
            else
            {
                movie.MainImg = MovieInDb.MainImg;
            }
            if(movievm.SubImageFiles != null)
            {
                foreach (var item in subimages)
                {
                    var oldfilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , item.SubImage);
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                }
                _context.MovieSubImages.RemoveRange(subimages);
                foreach (var item in movievm.SubImageFiles)
                {
                    var fileName = Guid.NewGuid().ToString() + "-" + item.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }
                    _context.MovieSubImages.Add(new MovieSubImage()
                    {
                        SubImage = fileName,
                        MovieId = movievm.Id
                    });
                }
            }
            _context.MovieActors.RemoveRange(actorsmovie.ToList());
            foreach (var item in movievm.ActorsId)
            {
                _context.MovieActors.Add(new MovieActor()
                {
                    MovieId = movievm.Id,
                    ActorId = item
                });
            }
            if (movievm.DateTime == new DateTime(new DateOnly(1, 1, 1), new TimeOnly(0, 0)))
            {
                movie.DateTime = MovieInDb.DateTime;
            }
            _context.Movies.Update(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == Id);
            var moviesubimages = _context.MovieSubImages.Where(c => c.MovieId == Id);
            if(movie!=null)
            {
                if (movie.MainImg != null)
                {
                    string oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\", movie.MainImg);
                    if (System.IO.File.Exists(oldfilepath))
                    {
                        System.IO.File.Delete(oldfilepath);
                    }
                }
                foreach(var item in moviesubimages)
                {
                    var oldfilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , item.SubImage);
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                }
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
