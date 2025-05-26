namespace MatrixApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int? CategorieId { get; set; }
        public string Naam { get; set; } = null!;
        public string? Beschrijving { get; set; }
        public decimal Prijs { get; set; }
        public int Voorraad {  get; set; }
        public byte[]? Afbeelding { get; set; }

        public Categorie? Categorie { get; set; }
        public ICollection<Bestelregel> Bestelregels { get; set; } = new List<Bestelregel>();
    }
}
