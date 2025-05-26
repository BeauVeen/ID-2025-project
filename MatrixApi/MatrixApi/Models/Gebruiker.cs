using System.Runtime.InteropServices;

namespace MatrixApi.Models
{
    public class Gebruiker
    {
        public int UserId { get; set; }
        public string Wachtwoord { get; set; } = null!;
        public bool Actief { get; set; }
        public string Naam { get; set; } = null!;
        public string? Adres { get; set; }
        public string? Postcode {get; set; }
        public string? Woonplaats{get; set; }
        public string? Telefoon { get; set; }
        public string? Email { get; set; }

        public ICollection<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();
    }
}
