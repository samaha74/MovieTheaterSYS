using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTheaterSYS.DataAccess;
using MovieTheaterSYS.Models;

namespace MovieTheaterSYS.Utilities
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbcontext _dbcontext;
        private readonly ILogger<DBInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public DBInitializer(ApplicationDbcontext dbcontext , ILogger<DBInitializer> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_dbcontext.Database.GetPendingMigrations().Any())
                {
                    _dbcontext.Database.Migrate();
                }
                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new(SD.SUPER_ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Employee_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Customer_ROLE)).GetAwaiter().GetResult();
                }

                _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "SuperAdmin@eraasoft.com",
                    UserName = "SuperAdmin",
                    Name = "SuperAdmin",
                    EmailConfirmed = true,
                }, "Admin@123").GetAwaiter().GetResult();
                var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.SUPER_ADMIN_ROLE).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "Admin@eraasoft.com",
                    UserName = "Admin",
                    Name = "Admin",
                    EmailConfirmed = true,
                }, "Admin@123").GetAwaiter().GetResult();
                user = _userManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.ADMIN_ROLE).GetAwaiter().GetResult();
                
                _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "Employee@eraasoft.com",
                    UserName = "Employee",
                    Name = "Employee",
                    EmailConfirmed = true,
                }, "Employee@123").GetAwaiter().GetResult();
                user = _userManager.FindByNameAsync("Employee").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.Employee_ROLE).GetAwaiter().GetResult();

               
                _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "Customer@eraasoft.com",
                    UserName = "Customer",
                    Name = "Customer",
                    EmailConfirmed = true,
                }, "Customer@123").GetAwaiter().GetResult();
                user = _userManager.FindByNameAsync("Customer").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.Customer_ROLE).GetAwaiter().GetResult();

            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
