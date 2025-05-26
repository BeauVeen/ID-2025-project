namespace MatrixApi.Models
{
    public class Bestelregel
    {
        public int BestelregelId { get; set; }
        public int BestelId { get; set; }
        public int ProductId { get; set; }
        public int Aantal {  get; set; }
        public decimal Prijs { get; set; }

        public Bestelling Bestelling { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
