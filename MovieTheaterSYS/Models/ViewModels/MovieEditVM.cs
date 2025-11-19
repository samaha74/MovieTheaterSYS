namespace MovieTheaterSYS.Models.ViewModels
{
    public class MovieEditVM
    {

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public bool status { get; set; }

        public int CategoryId { get; set; }
        public int CinemaId { get; set; }

        public string? MainImg { get; set; }

        public List<SubImgs> SubImages { get; set; } = new List<SubImgs>();

        public List<Actor> Actors { get; set; } = new List<Actor>();

        public List<Actor> CurrentActors { get; set; } = new List<Actor>();

        public List<int> SelectedActorIds { get; set; } = new List<int>();

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Cinema> Cinemas { get; set; } = new List<Cinema>();
    }
}
