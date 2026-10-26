using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    [PrimaryKey(nameof(MovieId) , nameof(SubImage))]
    public class MovieSubImage
    {
        [MaxLength(150)]
        public string SubImage { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
    }
}
