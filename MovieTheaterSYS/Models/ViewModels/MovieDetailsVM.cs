using MovieTheaterSYS.Areas.Admin.Controllers;

namespace MovieTheaterSYS.Models.ViewModels
{
    public class MovieDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string? MainImg{ get; set; }

        public DateTime Date { get; set; }
        public bool status { get; set; }
        // Relationships
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public List<SubImgs>? subImgs { get; set; }

        public List<Actor>? Actors { get; set; }

    }
}
