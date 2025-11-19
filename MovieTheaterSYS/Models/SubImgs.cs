namespace MovieTheaterSYS.Models
{
    public class SubImgs
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
