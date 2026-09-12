using System.ComponentModel.DataAnnotations;

namespace MvC_SystemMovies.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string image { get; set; } = string.Empty;

        public List<Movie>? Movies { get; set; }
    }
}
