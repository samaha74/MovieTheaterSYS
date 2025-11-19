using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MovieTheaterSYS.DataAccess;
using MovieTheaterSYS.Models;
using MovieTheaterSYS.Repository;
using MovieTheaterSYS.Utilities;

namespace MovieTheaterSYS
{
    public static class AppConfig
    {

        public static void config(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbcontext>(options =>

                options.UseSqlServer(connectionString)

            );


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            }
            )
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbcontext>();


            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = "/Identity/Account/login";
                o.AccessDeniedPath = "/Identity/Account/AccessDenied";
            }
            );


            services.AddTransient<IEmailSender, EmailSender>();


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<MovieActors>, Repository<MovieActors>>();
            services.AddScoped<IRepository<SubImgs>, Repository<SubImgs>>();

            services.AddScoped<IDBInitializer, DBInitializer>();

        }
    }
}
