using CinemaSystem.Data;

namespace CinemaSystem.Repositories
{
    public class MovieActorRepository : Repository<MovieActor> , IMovieActorRepository
    {
        public MovieActorRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void RemoveRange(IEnumerable<MovieActor> movieactors)
        {
            dbSet.RemoveRange(movieactors);
        }
    }
}
