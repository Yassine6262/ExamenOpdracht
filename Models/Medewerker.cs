namespace MedewerkersBeheerApp.Models
{
    public class Medewerker
    {
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Straat { get; set; }
        public string Postcode { get; set; }
        public string Plaats { get; set; }
        public bool IsDeleted { get; set; } = false; // Vlag voor verwijderde medewerkers
    }
}
