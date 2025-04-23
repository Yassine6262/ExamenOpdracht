using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamenOpdracht.Controllers
{
    public class LoginController : Controller
    {
        // Toont de loginpagina
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Verwerkt de loginform
        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            // Controleer of de gebruiker "admin" en wachtwoord "1234" heeft ingevuld
            if (username == "admin" && password == "1234")
            {
                // Maak een lijst met claims (wie de gebruiker is)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                // Maak een claims identity met cookie authenticatie
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Opties zoals sessieduur en persistentie
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                // Log de gebruiker in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Stuur door naar Homepagina
                return RedirectToAction("Index", "Home");
            }

            // Als login faalt, toon foutmelding
            ViewBag.Error = "Ongeldige inloggegevens. Vul gebruikersnaam: admin wachtwoord:1234 in.";
            return View();
        }

        // Uitlogfunctionaliteit
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Verwijder de login cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
