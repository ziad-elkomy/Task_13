using CinemaSystem.Data;

namespace CinemaSystem.Repositories
{
    public class MovieSubImageRepository : Repository<MovieSubImage> , IMovieSubImageRepository
    {
        public MovieSubImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void RemoveRange(IEnumerable<MovieSubImage> moviesubimages)
        {
            dbSet.RemoveRange(moviesubimages);
        }
    }
}
