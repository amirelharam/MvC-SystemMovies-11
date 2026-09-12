namespace MvC_SystemMovies.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public DateTime AddedOn { get; set; } = DateTime.Now;
    }
}
