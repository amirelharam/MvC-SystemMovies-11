using System.ComponentModel.DataAnnotations;

namespace MvC_SystemMovies.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public DateTime AddedOn { get; set; } = DateTime.Now;
    }
}
