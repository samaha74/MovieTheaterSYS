using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;
using System.Runtime.CompilerServices;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.Employee_ROLE}")]
    public class ActorController : Controller
    {
        IRepository<Actor> _ActorRepo;
        public ActorController(IRepository<Actor> repo)
        {
            _ActorRepo = repo;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _ActorRepo.GetAllAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (img != null && img.Length > 0)
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\Actors", img.FileName);
                if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\Actors")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\Actors"));
                }
                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }
            actor.Img = img.FileName;
            await _ActorRepo.AddAsync(actor);
            await _ActorRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize($"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task< IActionResult > Delete(int id)
        {
            var actor = await _ActorRepo.GetOneAsync(a => a.Id == id);
            if (actor != null)
            {
                _ActorRepo.Delete(actor);

                if (actor.Img != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\Actors", actor.Img);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                await _ActorRepo.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _ActorRepo.GetOneAsync(a => a.Id == id);
            return View(actor);
        }

        public async Task<IActionResult> Edit(Actor actor, IFormFile? img, string file)
        {

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
                return View(actor.Id);
            }

            actor.Img = file;
            if (img != null && img.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\Actors");
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
                actor.Img = uniqueFileName;
            }
            _ActorRepo.Update(actor);
            await _ActorRepo.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _ActorRepo.GetOneAsync(a => a.Id == id);
            return View(actor);
        }

    }
}
