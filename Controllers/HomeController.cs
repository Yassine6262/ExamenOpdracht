using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExamenOpdracht.Models;
using MedewerkersBeheerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;     

namespace ExamenOpdracht.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Aantal actieve medewerkers
            var medewerkersCount = await _context.Medewerkers
                .Where(m => !m.IsDeleted)
                .CountAsync();

            // Aantal verwijderde medewerkers
            var verwijderdeCount = await _context.Medewerkers
                .Where(m => m.IsDeleted)
                .CountAsync();

            // Gegevens doorgeven aan de View
            ViewData["MedewerkersCount"] = medewerkersCount;
            ViewData["VerwijderdeCount"] = verwijderdeCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
