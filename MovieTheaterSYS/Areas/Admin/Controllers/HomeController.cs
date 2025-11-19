using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE} , {SD.ADMIN_ROLE} , {SD.Employee_ROLE} ")]

    
    public class HomeController : Controller
    {

        IRepository<Movie> _movieRepo;

        IRepository<Category> _categoryRepo;
        IRepository<Cinema> _CinemaRepo;
        IRepository<Actor> _ActorRepo;

        public HomeController(IRepository<Movie> movierepo, IRepository<Category> categoryRepo, IRepository<Cinema> cinemarepo, IRepository<Actor> actorrepo)
        {
            _movieRepo = movierepo;
            _categoryRepo = categoryRepo;
            _CinemaRepo = cinemarepo;
            _ActorRepo = actorrepo;
        }

        public async Task<IActionResult> Index()
        {

            ViewBag.Movies = (await _movieRepo.GetAllAsync()).Count();
            ViewBag.Categories = (await _categoryRepo.GetAllAsync()).Count();
            ViewBag.Cinemas = (await _CinemaRepo.GetAllAsync()).Count();
            ViewBag.Actors = (await _ActorRepo.GetAllAsync()).Count();

            return View();
        }

    }
}
