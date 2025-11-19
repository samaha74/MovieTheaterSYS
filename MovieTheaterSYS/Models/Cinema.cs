namespace MovieTheaterSYS.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string location { get; set; }
        public string? img { get; set; }
        public bool status { get; set; }
        public ICollection<Movie>? Movies { get; set; }
    }
}
