namespace MvC_SystemMovies.Models
{
    public class Actors
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public List<MovieActors>? MovieActors { get; set; }
    }
}
