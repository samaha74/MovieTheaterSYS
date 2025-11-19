using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Models.ViewModels;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;

namespace MovieTheaterSYS.Areas.Identity.Controllers
{
        [Area("Identity")]
    public class Account : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<UserOTP> _userOTPRepository;

        public Account(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<UserOTP> userOTP)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userOTPRepository = userOTP;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {

                return View(registerVM);

            }

            var user = new ApplicationUser
            {
                UserName = registerVM.UserName,
                Name = registerVM.Name,
                Email = registerVM.EmailAddress,
            };

            var resault = await _userManager.CreateAsync(user , registerVM.Password);
            if (!resault.Succeeded) {

                foreach (var error in resault.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(registerVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail) , "Account", new {Area = "Identity", token, userId = user.Id}, Request.Scheme);
            await _emailSender.SendEmailAsync(registerVM.EmailAddress, "MovieTheater Confirmation Email",
                $"<h1>Confirm Your Email</h1><p> click <a href='{link}'>here</a> confirm your mail</p>");
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Invalid User";
            }

            var resault = await _userManager.ConfirmEmailAsync(user, token);
            if (!resault.Succeeded)
            {
                TempData["Error"] = "Fail to Confirm Email";
            }
            else
            {
                TempData["Success"] = "Email Confirmed!!";
            }

            return RedirectToAction(nameof(Login));

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Failed Login Atempt");
                return View(loginVM);
            }

            var resault = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!resault.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email Not Confirmed Yet");
                    return View(loginVM);
                }
                else if (resault.IsLockedOut)
                { 
                    ModelState.AddModelError(string.Empty, "Your Email is Locked Out. Please try again later");
                    return View(loginVM);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed Login Atempt");
                    return View(loginVM);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            bool iscustomer = true;

            foreach(var role in roles)
            {
                if (role != SD.Customer_ROLE)
                {
                    iscustomer = false;
                }
            }

            if (iscustomer)
                return RedirectToAction("Index", "Home" , new {area = "Customer"});

            return Redirect("/Admin");
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user with this email");
                return View(forgetPasswordVM);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm you Email first");
                return View(forgetPasswordVM);
            }

            var OTPs = await _userOTPRepository.GetAllAsync(o=> o.UserId == user.Id);
            var last24OTP = OTPs.Count(o=>o.CreatedDate >  DateTime.UtcNow.AddHours(-24));

            if(last24OTP > 10)
            {
                ModelState.AddModelError(string.Empty, "Too many atempts try agin later.");
                return View(forgetPasswordVM);
            }
            foreach(var o in OTPs)
            {
                o.isValid = false;
                _userOTPRepository.Update(o);
            }
            await _userOTPRepository.SaveChangesAsync();


            var OTP = new Random().Next(1000, 9999).ToString();
            var userOtp = new UserOTP(OTP,user.Id);
            await _userOTPRepository.AddAsync(userOtp);
            await _userOTPRepository.SaveChangesAsync();
            await _emailSender.SendEmailAsync(user.Email, "Foret Password OTP", $"<h4> you OTP is <span style='color:red;'>'{OTP}'</span></h4>");

            return RedirectToAction("ValidateOTP" , new { UserId = user.Id});
        }


        public IActionResult ValidateOTP(string UserId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var user = await _userManager.FindByIdAsync(validateOTPVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user found");
                return View(validateOTPVM.UserId);
            }

            var otp = await _userOTPRepository.GetOneAsync(o => o.UserId == user.Id && o.isValid && o.OTP == validateOTPVM.OTP);
            if(otp == null)
            {
                TempData["Error"] = "Invalid OTP";
                return View();
            }
            if(otp.ExpireDate < DateTime.UtcNow)
            {
                otp.isValid = false;
                _userOTPRepository.Update(otp);
                await _userOTPRepository.SaveChangesAsync();
                ModelState.AddModelError(string.Empty, "OTP Expired");
                return View(validateOTPVM);
            }
            otp.isValid = false;
            _userOTPRepository.Update(otp);
            await _userOTPRepository.SaveChangesAsync();
            TempData["ValidateUserId"] = user.Id;
            return RedirectToAction("ResetPassword" , new { UserId = user.Id });
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId)
        {
            var OTPValidUserId = TempData["ValidateUserId"]?.ToString();
            if(OTPValidUserId == null || OTPValidUserId != userId)
            {
                ModelState.AddModelError("", "Unauthorized Access to reset password");
                return RedirectToAction(nameof(ForgetPassword));
            }
            TempData.Keep("ValidateUserId");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var OTPValidUserId = TempData["ValidateUserId"]?.ToString();
            if (OTPValidUserId == null || OTPValidUserId != resetPasswordVM.UserId)
            {
                ModelState.AddModelError("", "Unauthorized Access to reset password");
                return RedirectToAction(nameof(ForgetPassword));
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid inputs");
                return View(resetPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(resetPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resault = await _userManager.ResetPasswordAsync(user, token,resetPasswordVM.Password);
            if(!resault.Succeeded)
            {
                foreach(var error in resault.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                    return View(resetPasswordVM);
            }

            TempData.Remove("ValidateUserId");

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index" , "Home" , new { area = "customer" });
        }

        [HttpGet]
        public IActionResult AccessDenied(string? retunUrl = null)
        {
            ViewBag.returnUrl = retunUrl;
            return View();
        }
    }
}
