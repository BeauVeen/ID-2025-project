namespace MatrixApi.Models
{
    public class Bestelling
    {
        public int BestelId { get; set; }
        public int UserId { get; set; }
        public DateTime Datum { get; set; }
        public string Status { get; set; } = null!;

        public Gebruiker Gebruiker { get; set; } = null!;
        public ICollection<Bestelregel> Bestelregels { get; set; } = new List<Bestelregel>();
    }
}
