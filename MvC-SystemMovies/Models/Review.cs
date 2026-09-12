using System.ComponentModel.DataAnnotations;

namespace MvC_SystemMovies.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
