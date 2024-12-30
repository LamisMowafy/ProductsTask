using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<ProductPromotion> ProductPromotion { get; set; }

        public DbSet<Users> Users { get; set; }

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

            modelBuilder.Entity<Promotion>()
                .Property(p => p.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            //modelBuilder.Entity<Products>().HasData(new Products
            //{
            //    Name = "",
            //    CreatedBy = 1,
            //    CreatedOn = DateTime.Now,
            //    Description = "",
            //    Price = 500
            //});
            //modelBuilder.Entity<Promotion>().HasData(new Promotion
            //{
            //    Code = "abc",
            //    DiscountPercentage = 10,
            //    Description = "",
            //    CreatedBy = 1,
            //    CreatedOn = DateTime.Now,
            //});

        }
    }
}
