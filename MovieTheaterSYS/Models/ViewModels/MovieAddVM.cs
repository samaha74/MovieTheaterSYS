namespace MovieTheaterSYS.Models.ViewModels
{
    public class MovieAddVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public DateTime Date { get; set; }
        public bool status { get; set; }
        // Relationships
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        
        public List<int>? SelectedActorIds { get; set; }
    }
}
