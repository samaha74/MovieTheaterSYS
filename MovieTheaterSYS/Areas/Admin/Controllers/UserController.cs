using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Utilities;
using System.Threading.Tasks;

namespace MovieTheaterSYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> LockUnlock(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {   
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(1);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
