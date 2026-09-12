using System.ComponentModel.DataAnnotations;

namespace MvC_SystemMovies.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        public bool Status { get; set; }

        public DateOnly DateTime { get; set; }

        public string? Minigm { get; set; }
        public string? Subimges { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

        public List<MovieActors>? MovieActors { get; set; }
    }
}
