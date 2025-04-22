using Microsoft.AspNetCore.Mvc;
using MedewerkersBeheerApp.Models;
using MedewerkersBeheerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace MedewerkersBeheerApp.Controllers
{
    [Authorize]
    public class MedewerkerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedewerkerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string zoekterm)
        {
            zoekterm = zoekterm?.Trim();
            var medewerkersQuery = _context.Medewerkers.Where(m => !m.IsDeleted);

            if (zoekterm == "")
            {
                ViewData["zoekterm"] = "";
                ViewData["leeg"] = "Voer een zoekterm in om te zoeken.";
                return View(new List<Medewerker>());
            }

            if (!string.IsNullOrEmpty(zoekterm))
            {
                medewerkersQuery = medewerkersQuery.Where(m =>
                    m.Voornaam.ToLower().Contains(zoekterm.ToLower()) ||
                    m.Achternaam.ToLower().Contains(zoekterm.ToLower())
                );

                ViewData["zoekterm"] = zoekterm;
            }

            var medewerkers = await medewerkersQuery.ToListAsync();

            if (!medewerkers.Any() && !string.IsNullOrEmpty(zoekterm))
            {
                ViewData["geenResultaten"] = "Geen medewerkers gevonden met de opgegeven zoekterm.";
            }

            return View(medewerkers);
        }

        public IActionResult Create()
        {
            return View(new Medewerker());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Voornaam,Achternaam,Straat,Postcode,Plaats")] Medewerker medewerker)
        {
            if (ModelState.IsValid)
            {
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medewerker = await _context.Medewerkers.FindAsync(id);
            if (medewerker == null) return NotFound();

            return View(medewerker);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var medewerker = await _context.Medewerkers.FindAsync(id);
            if (medewerker == null) return NotFound();

            medewerker.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var medewerker = await _context.Medewerkers.FindAsync(id);
            if (medewerker == null) return NotFound();

            medewerker.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerwijderdeMedewerkers));
        }

        public async Task<IActionResult> VerwijderdeMedewerkers()
        {
            var verwijderde = await _context.Medewerkers
                .Where(m => m.IsDeleted)
                .ToListAsync();

            return View(verwijderde);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreAll()
        {
            var verwijderde = await _context.Medewerkers
                .Where(m => m.IsDeleted)
                .ToListAsync();

            foreach (var m in verwijderde)
            {
                m.IsDeleted = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerwijderdeMedewerkers));
        }
    }
}
