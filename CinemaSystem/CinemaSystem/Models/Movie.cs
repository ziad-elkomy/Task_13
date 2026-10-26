using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string ?Description { get; set; }
        public bool Status { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        [MaxLength(150)]
        public string MainImg { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public int CinemaId { get; set; }
        [ForeignKey(nameof(CinemaId))]
        public Cinema Cinema { get; set; }
        
    }
}
