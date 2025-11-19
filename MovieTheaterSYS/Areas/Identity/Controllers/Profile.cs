using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Models.ViewModels;
using MovieTheaterSYS.Repository;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace MovieTheaterSYS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class Profile : Controller
    {
        UserManager<ApplicationUser> _userManager;

        public Profile(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userVM = user.Adapt<UserVM>();

            if (userVM == null)
            {
                return NotFound();
            }

            return View(userVM);
        }

        public async Task<IActionResult> Edit(UserVM userVM)
        {

            if (!ModelState.IsValid)
            {
                TempData["Errors"] = "Error";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = userVM.Name;
            user.UserName = userVM.UserName;
            user.Address = userVM.Address;

            var ret = await _userManager.UpdateAsync(user);
            if(!ret.Succeeded)
            {

                TempData["Error"] = String.Join(", ", ret.Errors.Select(e => e.Description));


                return RedirectToAction("Index");

            }
            else
            {
                TempData["Success"] = "Profile Updated Successfully";


                return View("Index", userVM);

            }


        }
    }
}
