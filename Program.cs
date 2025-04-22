using MedewerkersBeheerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Voeg de databasecontext toe (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Voeg cookie-authenticatie toe
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Loginpagina
        options.SlidingExpiration = true;
    });

// Voeg autorisatie toe
builder.Services.AddAuthorization();

// Voeg MVC-controllers met views toe
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ⛔️ Voorkom cache op alle pagina's
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

// Gebruik statische bestanden zoals CSS/JS uit wwwroot
app.UseStaticFiles();

// Gebruik authenticatie en autorisatie
app.UseAuthentication();
app.UseAuthorization();

// Voeg routing toe voor de applicatie
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
