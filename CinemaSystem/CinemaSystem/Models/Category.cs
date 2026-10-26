using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string ?Description { get; set; }
        public bool Status { get; set; }
    }
}
