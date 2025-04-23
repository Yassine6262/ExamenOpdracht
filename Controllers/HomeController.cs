using System.Diagnostics; 
using Microsoft.AspNetCore.Mvc; 
using ExamenOpdracht.Models; 
using MedewerkersBeheerApp.Data; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Authorization; 

namespace ExamenOpdracht.Controllers
{
    // Zorgt ervoor dat alleen ingelogde gebruikers deze controller kunnen gebruiken
    [Authorize]

    
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        // Databasecontext om toegang te krijgen tot de data
        private readonly ApplicationDbContext _context;

        
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Haalt gegevens op en toont de startpagina
        public async Task<IActionResult> Index()
        {
            // Tel het aantal medewerkers die NIET verwijderd zijn
            var medewerkersCount = await _context.Medewerkers
                .Where(m => !m.IsDeleted)
                .CountAsync();

            // Tel het aantal medewerkers die WEL verwijderd zijn
            var verwijderdeCount = await _context.Medewerkers
                .Where(m => m.IsDeleted)
                .CountAsync();

            // Doorsturen van deze tellingen naar de View via ViewData
            ViewData["MedewerkersCount"] = medewerkersCount;
            ViewData["VerwijderdeCount"] = verwijderdeCount;

            // Retourneer de standaard View voor deze actie (Index.cshtml)
            return View();
        }

        // Geeft de privacy-pagina weer
        public IActionResult Privacy()
        {
            return View();
        }

        // Foutpagina tonen met RequestId
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
