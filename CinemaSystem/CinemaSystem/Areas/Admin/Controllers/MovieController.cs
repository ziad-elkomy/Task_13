using CinemaSystem.Data;
using CinemaSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Movie> Movierepository; //= new Repository<Movie>();
        private readonly IRepository<Category> Categoryrepository; //= new Repository<Category>();
        private readonly IRepository<Cinema> Cinemarepository; //= new Repository<Cinema>();
        private readonly IRepository<Actor> Actorrepository; //= new Repository<Actor>();
        private readonly IMovieActorRepository MovieActorrepository; //= new MovieActorRepository();
        private readonly IMovieSubImageRepository MovieSubImagerepository; //= new MovieSubImageRepository();

        public MovieController(IRepository<Movie> movierepository, IRepository<Category> categoryrepository, IRepository<Cinema> cinemarepository, IMovieActorRepository movieActorrepository, IMovieSubImageRepository movieSubImagerepository, IRepository<Actor> actorrepository)
        {
            Movierepository = movierepository;
            Categoryrepository = categoryrepository;
            Cinemarepository = cinemarepository;
            MovieActorrepository = movieActorrepository;
            MovieSubImagerepository = movieSubImagerepository;
            Actorrepository = actorrepository;
        }

        public async Task<IActionResult> Index(FilterMovieVM filtermovie)
        {
            //var movies = _context.Movies.AsQueryable();
            var movies = await Movierepository.GetAllAsync(includes: [m => m.Category , m=>m.Cinema]);
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
            movies = movies.Skip((filtermovie.Page - 1) * 4).Take(4);
            ViewBag.TotalPages = Math.Ceiling(Movierepository.GetCount() / 4.0);
            return View(new FilterMovieVM()
            {
                Categories = await Categoryrepository.GetAllAsync(),
                Cinemas = await Cinemarepository.GetAllAsync(),
                Movies = movies,
                
                CategoryId = filtermovie.CategoryId,
                CinemaId = filtermovie.CinemaId,

                Price = filtermovie.Price,
                Status = filtermovie.Status,
                DT = filtermovie.DT,
                Page = filtermovie.Page

            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View(new MovieVM()
            {
                Categories = await Categoryrepository.GetAllAsync(),
                Cinemas = await Cinemarepository.GetAllAsync(),
                Actors = await Actorrepository.GetAllAsync()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieVM movievm)
        {
            string fileName ="";
            if (movievm.MainImageFile != null)
            {
                fileName = Guid.NewGuid().ToString() + "-" + movievm.MainImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    movievm.MainImageFile.CopyTo(stream);
                }
            }
            //var AddedMovie = _context.Movies.Add(movie);
            //_context.SaveChanges();
            var AddedMovie = await Movierepository.AddAsync(new Movie()
            {
                Name = movievm.Name,
                Description = movievm.Description,
                CinemaId = movievm.CinemaId,
                CategoryId = movievm.CategoryId,
                Price = movievm.Price,
                DateTime = movievm.DateTime,
                Status = movievm.Status,
                MainImg = fileName
            });
            await Movierepository.SaveChangesAsync();
            if(movievm.SubImageFiles != null)
            {
                foreach (var item in movievm.SubImageFiles)
                {
                    var fileName1 = Guid.NewGuid().ToString() + "-" + item.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\", fileName1);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }
                    await MovieSubImagerepository.AddAsync(new MovieSubImage()
                    {
                        SubImage = fileName1,
                        MovieId = AddedMovie.Entity.Id
                    });
                }
                await MovieSubImagerepository.SaveChangesAsync();
            }
            
            if(movievm.MovieActorsIds != null)
            {
                foreach (var item in movievm.MovieActorsIds)
                {
                    await MovieActorrepository.AddAsync(new MovieActor()
                    {
                        MovieId = AddedMovie.Entity.Id,
                        ActorId = item
                    });
                }
                //_context.SaveChanges();
                await MovieActorrepository.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            //var movie = _context.Movies.FirstOrDefault(c => c.Id == Id);
            var movie = await Movierepository.GetFirstAsync(c => c.Id == Id);


            return View(new MovieVM()
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Status = movie.Status,
                DateTime = movie.DateTime,
                Price = movie.Price,
                MainImg = movie.MainImg,
                Categories = await Categoryrepository.GetAllAsync(),
                CategoryId = movie.CategoryId,
                Cinemas = await Cinemarepository.GetAllAsync(),
                CinemaId = movie.CinemaId,
                Actors = await Actorrepository.GetAllAsync(),
                MovieActors = await MovieActorrepository.GetAllAsync(c => c.MovieId == movie.Id),
                SubImages = await MovieSubImagerepository.GetAllAsync(c=> c.MovieId == movie.Id)
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(MovieVM movievm)
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
            //var MovieInDb = _context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == movievm.Id);
            //var subimages = _context.MovieSubImages.Where(c => c.MovieId == movievm.Id);
            //var actorsmovie = _context.MovieActors.AsNoTracking().Where(c => c.MovieId == c.MovieId);
            var MovieInDb = await Movierepository.GetFirstAsync(m => m.Id == movievm.Id, isTracked: false);
            var subimages = await MovieSubImagerepository.GetAllAsync(m => m.MovieId == movievm.Id);
            var actorsmovie = await MovieActorrepository.GetAllAsync(m => m.MovieId == movievm.Id , isTracked:false);
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
                //_context.MovieSubImages.RemoveRange(subimages);//****/
                MovieSubImagerepository.RemoveRange(subimages);
                foreach (var item in movievm.SubImageFiles)
                {
                    var fileName = Guid.NewGuid().ToString() + "-" + item.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Img\\Admin\\Movie\\" , fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }
                    await MovieSubImagerepository.AddAsync(new MovieSubImage()
                    {
                        SubImage = fileName,
                        MovieId = movievm.Id
                    });
                }
            }
            //_context.MovieActors.RemoveRange(actorsmovie.ToList());//****/
            MovieActorrepository.RemoveRange(actorsmovie);
            foreach (var item in movievm.MovieActors)
            {
                await MovieActorrepository.AddAsync(new MovieActor()

                {
                    MovieId = movievm.Id,
                    ActorId = item.ActorId
                });
            }
            if (movievm.DateTime == new DateTime(new DateOnly(1, 1, 1), new TimeOnly(0, 0)))
            {
                movie.DateTime = MovieInDb.DateTime;
            }
            //_context.Movies.Update(movie);
            //_context.SaveChanges();
            Movierepository.Update(movie);
            await Movierepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            //var movie = _context.Movies.FirstOrDefault(c => c.Id == Id);
            //var moviesubimages = _context.MovieSubImages.Where(c => c.MovieId == Id);
            //var movieactors = _context.MovieActors.Where(c=> c.MovieId == Id);
            var movie = await Movierepository.GetFirstAsync(c => c.Id == Id);
            var moviesubimages = await MovieSubImagerepository.GetAllAsync(c => c.MovieId == Id);
            var movieactors = await MovieActorrepository.GetAllAsync(c => c.MovieId == Id);
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
                MovieActorrepository.RemoveRange(movieactors);
                MovieSubImagerepository.RemoveRange(moviesubimages);
                //_context.Movies.Remove(movie);
                //_context.SaveChanges();
                Movierepository.Remove(movie);
                await Movierepository.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
