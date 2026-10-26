using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [MaxLength(150)]
        public string ?Description { get; set; }
        public bool Status { get; set; }
        [MaxLength(150)]
        public string Image { get; set; }
    }
}
