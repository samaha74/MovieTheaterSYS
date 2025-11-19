using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.Employee_ROLE}")]
    public class CinemaController : Controller
    {
        IRepository<MovieTheaterSYS.Models.Cinema> _cinemaRepository;
        public CinemaController(IRepository<MovieTheaterSYS.Models.Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }
        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetAllAsync();
            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile img)
        {
            Console.WriteLine("✅ POST Create() hit!");
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key; // the property name
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Property: {key}, Error: {error.ErrorMessage}");
                    }
                }
                Console.WriteLine("✅ POST Create() hit!");
                ModelState.AddModelError("" , "");
                return View(cinema);
            }

            if (img != null && img.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\cinemas");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }
                cinema.img = uniqueFileName;
            }

            await _cinemaRepository.AddAsync(cinema);
            await _cinemaRepository.SaveChangesAsync();
            Console.WriteLine("✅ POST Create() hit!");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? img , string file)
        {
            
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            cinema.img = file;
            if (img != null && img.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\cinemas");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }
                var existingFilePath = Path.Combine(uploadsDir, file);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                cinema.img = uniqueFileName;
            }
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [Authorize($"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(cinema.img))
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\cinemas");
                var existingFilePath = Path.Combine(uploadsDir, cinema.img);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);

        }
    }
}
