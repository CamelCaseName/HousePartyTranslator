using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TranslatorWebUser.Areas.Identity;
using TranslatorWebUser.Data;

namespace TranslatorWebUser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            _ = builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            _ = builder.Services.AddRazorPages();
            _ = builder.Services.AddServerSideBlazor();
            _ = builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            _ = builder.Services.AddSingleton<WeatherForecastService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseMigrationsEndPoint();
            }
            else
            {
                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseStaticFiles();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.MapControllers();
            _ = app.MapBlazorHub();
            _ = app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}