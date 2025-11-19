using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Models.ViewModels;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.Employee_ROLE}")]
    public class MovieController : Controller
    {
        IRepository<Movie> _MovieRepo;
        IRepository<SubImgs> _SubImgRepo;
        IRepository<Category> _categoryReop;
        IRepository<Actor> _actorRepo;
        IRepository<Cinema> _cinemaRepo;
        IRepository<MovieActors> _movieActorRepo;


        public MovieController(IRepository<Movie> repo, IRepository<SubImgs> subImgRepo, IRepository<Category> categoryReop, IRepository<Actor> actorRepo, IRepository<Cinema> cinemaRepo, IRepository<MovieActors> movieActorRepo)
        {
            _MovieRepo = repo;
            _SubImgRepo = subImgRepo;
            _categoryReop = categoryReop;
            _actorRepo = actorRepo;
            _cinemaRepo = cinemaRepo;
            _movieActorRepo = movieActorRepo;
        }
        public async Task<IActionResult> Index()
        {
            var items = await _MovieRepo.GetAllAsync(joins: [m=>m.Category , m=>m.Cinema]);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult >Create(Movie? movie)
        {
            ViewBag.movie = movie;
            ViewBag.categories= await _categoryReop.GetAllAsync();
            ViewBag.actors = await _actorRepo.GetAllAsync();
            ViewBag.cinemas = await _cinemaRepo.GetAllAsync();

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieAddVM movieAdd, IFormFile img, IFormFile[]? subimgs)
        {
            if (!ModelState.IsValid)
            {
                return View(movieAdd);
            }

            if (img != null && img.Length > 0)
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies", img.FileName);

                if(!Directory.Exists(Path.GetDirectoryName(imgPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(imgPath));
                }

                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }

            Movie movie = new Movie()
            {
                Name = movieAdd.Name,
                Description = movieAdd.Description,
                Price = movieAdd.Price,
                Date = movieAdd.Date,
                status = movieAdd.status,
                CategoryId = movieAdd.CategoryId,
                CinemaId = movieAdd.CinemaId,
                MainImg = img.FileName
            };
            var currentMovie =  await _MovieRepo.AddAsync(movie);
            await _MovieRepo.SaveChangesAsync();

            if(movieAdd.SelectedActorIds != null && movieAdd.SelectedActorIds.Count > 0)
            {
                foreach (var actorId in movieAdd.SelectedActorIds)
                {
                    MovieActors movieActor = new MovieActors()
                    {
                        MovieId = currentMovie.Entity.Id,
                        ActorId = actorId
                    };
                    await _movieActorRepo.AddAsync(movieActor);
                }
                await _movieActorRepo.SaveChangesAsync();
            }

            if (subimgs != null && subimgs.Length != 0)
            {
                var subimgsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies/subimgs");

                if (!Directory.Exists(subimgsPath))
                {
                    Directory.CreateDirectory(subimgsPath);
                }

                foreach (var subimg in subimgs)
                {
                    if (subimg != null && subimg.Length > 0)
                    {
                        var fullPath = Path.Combine(subimgsPath, subimg.FileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await subimg.CopyToAsync(stream);
                        }
                        SubImgs subImage = new SubImgs()
                        {
                            MovieId = currentMovie.Entity.Id,
                            Img = subimg.FileName
                        };
                        await _SubImgRepo.AddAsync(subImage);
                    }
                }
                await _SubImgRepo.SaveChangesAsync();
            }
            currentMovie.Entity.MainImg = img.FileName;

            await _MovieRepo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Load movie + its actor relations
            var movie = await _MovieRepo.GetOneAsync(
                m => m.Id == id,
                joins: [
                    m => m.movieActors
                ]
            );

            if (movie == null)
                return NotFound();

            // Load extra collections
            var allActors = await _actorRepo.GetAllAsync();
            var categories = await _categoryReop.GetAllAsync();
            var cinemas = await _cinemaRepo.GetAllAsync();
            var subImages = await _SubImgRepo.GetAllAsync(s => s.MovieId == id);

            // Build the viewmodel
            var vm = new MovieEditVM
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Date = movie.Date,
                status = movie.status,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                MainImg = movie.MainImg,

                // lists for dropdowns
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = allActors.ToList(),

                // current selected actors
                CurrentActors = movie.movieActors
                    .Select(ma => ma.Actor)
                    .ToList(),

                // preselected IDs for Select2
                SelectedActorIds = movie.movieActors
                    .Select(ma => ma.ActorId)
                    .ToList(),

                // subimages preview
                SubImages = subImages.ToList()
            };

            return View(vm);
        }


        [HttpPost]


        [HttpPost]
        public async Task<IActionResult> Edit(MovieEditVM vm, IFormFile? img, IFormFile[]? subimgs)
        {
            if (!ModelState.IsValid)
            {
                vm.Actors = (await _actorRepo.GetAllAsync()).ToList();
                vm.Categories = (await _categoryReop.GetAllAsync()).ToList();
                vm.Cinemas = (await _cinemaRepo.GetAllAsync()).ToList();

                var movie = await _MovieRepo.GetOneAsync(m => m.Id == vm.Id);
                if (movie != null)
                {
                    vm.SubImages = (await _SubImgRepo.GetAllAsync(s => s.MovieId == movie.Id)).ToList();
                    vm.CurrentActors = (await _movieActorRepo.GetAllAsync(ma => ma.MovieId == movie.Id, joins: [ma => ma.Actor]))
                                        .Select(ma => ma.Actor).ToList();
                }

                return View(vm);
            }

            // Get movie from repo
            var dbMovie = await _MovieRepo.GetOneAsync(m => m.Id == vm.Id);
            if (dbMovie == null)
                return NotFound();

            // --- Update basic fields ---
            dbMovie.Name = vm.Name;
            dbMovie.Description = vm.Description;
            dbMovie.Price = vm.Price;
            dbMovie.Date = vm.Date;
            dbMovie.status = vm.status;
            dbMovie.CategoryId = vm.CategoryId;
            dbMovie.CinemaId = vm.CinemaId;

            // --- Update main image ---
            if (img != null && img.Length > 0)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(dbMovie.MainImg))
                {
                    var oldPath = Path.Combine("wwwroot/uploads/movies", dbMovie.MainImg);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var imgPath = Path.Combine("wwwroot/uploads/movies", img.FileName);
                if (!Directory.Exists(Path.GetDirectoryName(imgPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(imgPath));

                using (var stream = new FileStream(imgPath, FileMode.Create))
                    await img.CopyToAsync(stream);

                dbMovie.MainImg = img.FileName;
            }

            // --- Add new sub-images ---
            if (subimgs != null && subimgs.Length > 0)
            {
                var subImgsPath = Path.Combine("wwwroot/uploads/movies/subimgs");
                if (!Directory.Exists(subImgsPath))
                    Directory.CreateDirectory(subImgsPath);

                foreach (var subimg in subimgs)
                {
                    if (subimg.Length > 0)
                    {
                        var fullPath = Path.Combine(subImgsPath, subimg.FileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                            await subimg.CopyToAsync(stream);

                        await _SubImgRepo.AddAsync(new SubImgs
                        {
                            MovieId = dbMovie.Id,
                            Img = subimg.FileName
                        });
                    }
                }
                await _SubImgRepo.SaveChangesAsync();
            }

            // --- Update MovieActors ---
            var oldActors = await _movieActorRepo.GetAllAsync(ma => ma.MovieId == dbMovie.Id);
            foreach (var ma in oldActors)
                _movieActorRepo.Delete(ma);

            if (vm.SelectedActorIds != null && vm.SelectedActorIds.Count > 0)
            {
                foreach (var actorId in vm.SelectedActorIds)
                {
                    await _movieActorRepo.AddAsync(new MovieActors
                    {
                        MovieId = dbMovie.Id,
                        ActorId = actorId
                    });
                }
                await _movieActorRepo.SaveChangesAsync();
            }

            // --- Save movie changes ---
            _MovieRepo.Update(dbMovie);
            await _MovieRepo.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        //public async Task<IActionResult> Edit(MovieEditVM movieEdit, IFormFile? img, IFormFile[]? subimgs , string file)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(movieEdit);
        //    }

        //    var movie = new Movie()
        //    {
        //        Id = movieEdit.Id,
        //        Name = movieEdit.Name,
        //        Description = movieEdit.Description,
        //        Price = movieEdit.Price,
        //        Date = movieEdit.Date,
        //        status = movieEdit.status,
        //        CategoryId = movieEdit.CategoryId,
        //        CinemaId = movieEdit.CinemaId
        //    };


        //    movie.MainImg = file;
        //    if (img != null && img.Length > 0)
        //    {
        //        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies", img.FileName);
        //        if (!Directory.Exists(Path.GetDirectoryName(imgPath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(imgPath));
        //        }
        //        using (var stream = new FileStream(imgPath, FileMode.Create))
        //        {
        //            await img.CopyToAsync(stream);
        //        }

        //        var existingImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies", movie.MainImg);
        //        if (System.IO.File.Exists(existingImagePath))
        //        {
        //            System.IO.File.Delete(existingImagePath);
        //        }

        //        movie.MainImg = img.FileName;

        //    }

        //    if(subimgs != null && subimgs.Length > 0)
        //    {
        //        var subimgsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies/subimgs");
        //        if (!Directory.Exists(subimgsPath))
        //        {
        //            Directory.CreateDirectory(subimgsPath);
        //        }
        //        foreach (var subimg in subimgs)
        //        {
        //            if (subimg != null && subimg.Length > 0)
        //            {
        //                var fullPath = Path.Combine(subimgsPath, subimg.FileName);
        //                using (var stream = new FileStream(fullPath, FileMode.Create))
        //                {
        //                    await subimg.CopyToAsync(stream);
        //                }
        //                SubImgs subImage = new SubImgs()
        //                {
        //                    MovieId = movie.Id,
        //                    Img = subimg.FileName
        //                };
        //                await _SubImgRepo.AddAsync(subImage);
        //            }
        //        }
        //        await _SubImgRepo.SaveChangesAsync();
        //    }




        //    _MovieRepo.Update(movie);
        //    await _MovieRepo.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        [Authorize($"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _MovieRepo.GetOneAsync(m=>m.Id == id);
            if(movie != null)
            {
                 _MovieRepo.Delete(movie);

                if(!string.IsNullOrEmpty( movie.MainImg))
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies", movie.MainImg);
                    if (System.IO.File.Exists(imgPath))
                    {
                        System.IO.File.Delete(imgPath);
                    }

                    var subImages = await _SubImgRepo.GetAllAsync(s => s.MovieId == movie.Id);
                    foreach (var subImg in subImages)
                    {
                        var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies/subimgs", subImg.Img);
                        if (System.IO.File.Exists(subImgPath))
                        {
                            System.IO.File.Delete(subImgPath);
                        }
                        _SubImgRepo.Delete(subImg);
                    }
                }

                var movieActors = await _movieActorRepo.GetAllAsync(ma => ma.MovieId == movie.Id);
                foreach (var movieActor in movieActors)
                {
                    _movieActorRepo.Delete(movieActor);
                }

                await _MovieRepo.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [Authorize($"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteSubImg(int id)
        {
                       var subImg = await _SubImgRepo.GetOneAsync(s => s.Id == id);
            if(subImg != null)
            {
                var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/movies/subimgs", subImg.Img);
                if (System.IO.File.Exists(subImgPath))
                {
                    System.IO.File.Delete(subImgPath);
                }
                _SubImgRepo.Delete(subImg);
                await _SubImgRepo.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = subImg.MovieId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _MovieRepo.GetOneAsync(m => m.Id == id , [m=>m.Cinema , m=>m.Category]);
            if (movie == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.subImgs = await _SubImgRepo.GetAllAsync(s=>s.MovieId == id);

            //var actorsM = (await _movieActorRepo.GetAllAsync(a => a.MovieId == id));

            var actors = new List<Actor>();

            foreach (var i in (await _movieActorRepo.GetAllAsync(a => a.MovieId == id)))
            {
                actors.Add(await _actorRepo.GetOneAsync(a=>a.Id == i.ActorId));
            }

            ViewBag.actors = actors;



            return View(movie);
        }
    }
}
