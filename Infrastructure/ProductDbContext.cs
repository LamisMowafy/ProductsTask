using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options) { }

    public DbSet<Products> Products { get; set; }
    public DbSet<Promotions> Promotions { get; set; }
    public DbSet<ProductPromotion> ProductPromotion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductPromotion>().HasKey(pp => new { pp.ProductId, pp.PromotionId });

        modelBuilder.Entity<ProductPromotion>()
            .HasOne(pp => pp.Product)
            .WithMany(p => p.ProductPromotions)
            .HasForeignKey(pp => pp.ProductId);

        modelBuilder.Entity<ProductPromotion>()
            .HasOne(pp => pp.Promotion)
            .WithMany(p => p.ProductPromotions)
            .HasForeignKey(pp => pp.PromotionId);

        modelBuilder.Entity<Products>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Promotions>()
            .Property(p => p.DiscountPercentage)
            .HasColumnType("decimal(5,2)");
    }
}
