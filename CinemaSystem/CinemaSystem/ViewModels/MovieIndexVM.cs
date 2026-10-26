namespace CinemaSystem.ViewModels
{
    public class MovieIndexVM
    {
        public List<Movie> Movies { get; set; }
        public List<Category> Categories { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public FilterMovieVM filter { get; set; }

    }
}
