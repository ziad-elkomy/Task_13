using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    [PrimaryKey(nameof(ActorId) , nameof(MovieId))]
    public class MovieActor
    {
        public int ActorId { get; set;}
        [ForeignKey(nameof(ActorId))]
        public Actor Actor { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
    }
}
