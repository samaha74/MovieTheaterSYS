using Microsoft.AspNetCore.Mvc;

namespace MovieTheaterSYS.Models
{

    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string? Img { get; set; }
        public bool status { get; set; }

        public ICollection<MovieActors>? movieActors { get; set; }
    }
}
