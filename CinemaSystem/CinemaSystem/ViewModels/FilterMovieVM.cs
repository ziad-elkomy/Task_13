namespace CinemaSystem.ViewModels
{
    public class FilterMovieVM
    {
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public DateTime DT { get; set; }
        public bool ?Status { get; set; }
        public decimal Price { get; set; }
        public int Page { get; set; } = 1;
    }
}
