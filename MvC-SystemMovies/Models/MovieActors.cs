namespace MvC_SystemMovies.Models
{
    public class MovieActors
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public int ActorId { get; set; }
        public Actors? Actor { get; set; }
    }
}
