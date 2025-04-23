using Microsoft.AspNetCore.Mvc;
using MedewerkersBeheerApp.Models;
using MedewerkersBeheerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace MedewerkersBeheerApp.Controllers
{
    [Authorize] // Zorgt ervoor dat alleen geautoriseerde gebruikers toegang hebben tot deze controller
    public class MedewerkerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor om de context in te stellen
        public MedewerkerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index actie, toont medewerkers op basis van zoekterm
        public async Task<IActionResult> Index(string zoekterm)
        {
            zoekterm = zoekterm?.Trim(); 
            var medewerkersQuery = _context.Medewerkers.Where(m => !m.IsDeleted); 

            // Controleert of zoekterm leeg is
            if (zoekterm == "")
            {
                ViewData["zoekterm"] = "";
                ViewData["leeg"] = "Voer een zoekterm in om te zoeken.";
                return View(new List<Medewerker>()); 
            }

            // Als zoekterm niet leeg is, filtert medewerkers op voornaam of achternaam
            if (!string.IsNullOrEmpty(zoekterm))
            {
                medewerkersQuery = medewerkersQuery.Where(m =>
                    m.Voornaam.ToLower().Contains(zoekterm.ToLower()) || 
                    m.Achternaam.ToLower().Contains(zoekterm.ToLower())
                );

                ViewData["zoekterm"] = zoekterm;
            }

            // Voert de query uit en haalt de resultaten op
            var medewerkers = await medewerkersQuery.ToListAsync();

            // Als geen medewerkers gevonden zijn, wordt een boodschap weergegeven
            if (!medewerkers.Any() && !string.IsNullOrEmpty(zoekterm))
            {
                ViewData["geenResultaten"] = "Geen medewerkers gevonden met de opgegeven zoekterm.";
            }

            return View(medewerkers);
        }

        // Actie om een nieuwe medewerker te creëren
        public IActionResult Create()
        {
            return View(new Medewerker()); 
        }

        // POST actie voor het creëren van een nieuwe medewerker
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Voornaam,Achternaam,Straat,Postcode,Plaats")] Medewerker medewerker)
        {
            if (ModelState.IsValid)
            {
                // Controleert of een medewerker met dezelfde voornaam en achternaam al bestaat
                var bestaat = await _context.Medewerkers
                    .AnyAsync(m => m.Voornaam == medewerker.Voornaam && m.Achternaam == medewerker.Achternaam);

                if (bestaat)
                {
                    ModelState.AddModelError("", "Een medewerker met dezelfde voornaam en achternaam bestaat al!"); 
                    return View(medewerker);
                }

                _context.Add(medewerker); 
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index)); 
            }

            return View(medewerker); 
        }

        // Actie om een medewerker te bewerken
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound(); 

            var medewerker = await _context.Medewerkers.FindAsync(id); 
            if (medewerker == null) return NotFound(); 

            return View(medewerker); 
        }

        // POST actie voor het bewerken van een medewerker
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,Voornaam,Achternaam,Straat,Postcode,Plaats")] Medewerker medewerker)
        {
            if (id != medewerker.Id) return NotFound(); 

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medewerker); 
                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException) 
                {
                    if (!_context.Medewerkers.Any(m => m.Id == medewerker.Id))
                        return NotFound(); 
                    else
                        throw; 
                }

                return RedirectToAction(nameof(Index)); 
            }

            return View(medewerker); 
        }

        // Actie om een medewerker te verwijderen (logisch verwijderen door IsDeleted op true te zetten)
        public async Task<IActionResult> Delete(int id)
        {
            var medewerker = await _context.Medewerkers.FindAsync(id); // Zoek de medewerker
            if (medewerker == null) return NotFound(); // Retourneer fout als medewerker niet bestaat

            medewerker.IsDeleted = true; // Zet IsDeleted op true om de medewerker te markeren als verwijderd
            await _context.SaveChangesAsync(); // Sla de wijzigingen op
            return RedirectToAction(nameof(Index)); // Redirect naar de Index pagina
        }

        // POST actie om een verwijderde medewerker te herstellen
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var medewerker = await _context.Medewerkers.FindAsync(id); // Zoek de medewerker
            if (medewerker == null) return NotFound(); // Retourneer fout als medewerker niet bestaat

            medewerker.IsDeleted = false; // Zet IsDeleted op false om de medewerker te herstellen
            await _context.SaveChangesAsync(); // Sla de wijzigingen op
            return RedirectToAction(nameof(VerwijderdeMedewerkers)); // Redirect naar de lijst van verwijderde medewerkers
        }

        // Actie om verwijderde medewerkers weer te geven
        public async Task<IActionResult> VerwijderdeMedewerkers()
        {
            var verwijderde = await _context.Medewerkers
                .Where(m => m.IsDeleted) // Haalt alleen de medewerkers die verwijderd zijn op
                .ToListAsync();

            return View(verwijderde); 
        }

        // POST actie om alle verwijderde medewerkers te herstellen
        [HttpPost]
        public async Task<IActionResult> RestoreAll()
        {
            var verwijderde = await _context.Medewerkers
                .Where(m => m.IsDeleted) // Haalt alle verwijderde medewerkers op
                .ToListAsync();

            // Herstel alle verwijderde medewerkers
            foreach (var m in verwijderde)
            {
                m.IsDeleted = false;
            }

            await _context.SaveChangesAsync(); // Sla de wijzigingen op
            return RedirectToAction(nameof(VerwijderdeMedewerkers)); // Redirect naar de lijst van verwijderde medewerkers
        }
    }
}
