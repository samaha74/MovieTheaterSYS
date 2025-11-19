
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Models.ViewModels;

namespace MovieTheaterSYS.DataAccess
{
    public class ApplicationDbcontext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {
        }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<Actor> actors { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<MovieActors> movieActors { get; set; }
        public DbSet<SubImgs> subImgs { get; set; }
        public DbSet<UserOTP> userOTPs { get; set; }

        ///////////////////////////////////////////////////////////





    }
}
