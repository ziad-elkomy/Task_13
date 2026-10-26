using System.ComponentModel.DataAnnotations;
namespace CinemaSystem.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string ?Description { get; set; }
        [MaxLength(100)]
        public string Image { get; set; }
    }
}
