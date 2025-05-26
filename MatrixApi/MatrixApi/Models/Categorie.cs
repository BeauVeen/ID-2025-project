namespace MatrixApi.Models
{
    public class Categorie
    {
        public int CategorieId { get; set; }
        public string CategorieNaam { get; set; } = null!;

        public ICollection<Product> Producten { get; set; } = new List<Product>();
    }
}
