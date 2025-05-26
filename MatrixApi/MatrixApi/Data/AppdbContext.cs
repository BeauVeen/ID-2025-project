using Microsoft.EntityFrameworkCore;
using MatrixApi.Models;

namespace MatrixApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Gebruiker> Gebruikers {  get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Product> Producten { get; set; }
        public DbSet<Bestelling> Bestellingen { get; set; }
        public DbSet<Bestelregel> Bestelregels {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gebruiker>().ToTable("Gebruiker");
            modelBuilder.Entity<Categorie>().ToTable("Categorie");
            modelBuilder.Entity<Product>().ToTable("Producten");
            modelBuilder.Entity<Bestelling>().ToTable("Bestelling");
            modelBuilder.Entity<Bestelregel>().ToTable("Bestelregel");

            modelBuilder.Entity<Bestelling>()
                .HasOne(b => b.Gebruiker)
                .WithMany(g => g.Bestellingen)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Categorie)
                .WithMany(c => c.Producten)
                .HasForeignKey(p => p.CategorieId);

            modelBuilder.Entity<Bestelregel>()
                .HasOne(br => br.Bestelling)
                .WithMany(b => b.Bestelregels)
                .HasForeignKey(br => br.BestelId);

            modelBuilder.Entity<Bestelregel>()
                .HasOne(br => br.Product)
                .WithMany(p => p.Bestelregels)
                .HasForeignKey(br => br.ProductId);

            modelBuilder.Entity<Bestelling>().HasKey(b => b.BestelId);
            modelBuilder.Entity<Gebruiker>().HasKey(g => g.UserId);
            modelBuilder.Entity<Categorie>().HasKey(c => c.CategorieId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Bestelregel>().HasKey(br => br.BestelregelId);
        }
    }
}
