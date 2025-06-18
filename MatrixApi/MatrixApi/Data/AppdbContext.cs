using Microsoft.EntityFrameworkCore;
using MatrixApi.Models;

namespace MatrixApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users {  get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Orderline> Orderlines {  get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContainerOrder> ContainerOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Orderline>().ToTable("Orderlines");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Container>().ToTable("Container");
            modelBuilder.Entity<ContainerOrder>().ToTable("ContainerOrders");

            modelBuilder.Entity<Order>()
                .HasOne(b => b.User)
                .WithMany(g => g.Orders)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Orderline>()
                .HasOne(br => br.Order)
                .WithMany(b => b.Orderlines)
                .HasForeignKey(br => br.OrderId);

            modelBuilder.Entity<Orderline>()
                .HasOne(br => br.Product)
                .WithMany(p => p.Orderlines)
                .HasForeignKey(br => br.ProductId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<ContainerOrder>()
                .HasKey(co => co.ContainerOrderId);

            modelBuilder.Entity<ContainerOrder>()
                .HasOne(co => co.Container)
                .WithMany(c => c.ContainerOrders)
                .HasForeignKey(co => co.ContainerId);

            modelBuilder.Entity<ContainerOrder>()
                .HasOne(co => co.Order)
                .WithMany()
                .HasForeignKey(co => co.OrderId);

            modelBuilder.Entity<ContainerOrder>()
                .HasIndex(co => new { co.ContainerId, co.OrderId })
                .IsUnique();

            modelBuilder.Entity<Order>().HasKey(b => b.OrderId);
            modelBuilder.Entity<User>().HasKey(g => g.UserId);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Orderline>().HasKey(br => br.OrderlineId);
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId);
            modelBuilder.Entity<Container>().HasKey(c => c.ContainerId);
        }
    }
}
