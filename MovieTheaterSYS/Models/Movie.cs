namespace MovieTheaterSYS.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string? MainImg { get; set; }

        public DateTime Date { get; set; }
        public bool status { get; set; }
        // Relationships
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public ICollection<SubImgs>? SubImgs { get; set; }
        public ICollection<MovieActors>? movieActors{ get; set; }
    }
}
