namespace CinemaSystem.Repositories
{
    public interface IMovieActorRepository : IRepository<MovieActor>
    {
        public void RemoveRange(IEnumerable<MovieActor> movieActors);
    }
}
