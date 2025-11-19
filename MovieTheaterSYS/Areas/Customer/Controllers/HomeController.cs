using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using System.Threading.Tasks;

namespace MovieTheaterSYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        IRepository<Movie> _repository;

        public HomeController(IRepository<Movie> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _repository.GetAllAsync();

            return View(movies);
        }
    }
}
