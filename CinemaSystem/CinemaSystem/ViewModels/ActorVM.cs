namespace CinemaSystem.ViewModels
{
    public class ActorVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Page { get; set; } = 1;
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
