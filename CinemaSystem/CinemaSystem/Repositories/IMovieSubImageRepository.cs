namespace CinemaSystem.Repositories
{
    public interface IMovieSubImageRepository : IRepository<MovieSubImage>
    {
        public void RemoveRange(IEnumerable<MovieSubImage> movieSubImages);
    }
}
