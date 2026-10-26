using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        public bool Status { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        public string MainImg { get; set; }
        public IFormFile MainImageFile { get; set; }
        public List<IFormFile> SubImageFiles { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public List<int> ActorsId { get; set; }
    }
}
