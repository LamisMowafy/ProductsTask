using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : Repository<Products>, IProductRepository
    {
        public ProductRepository(ProductDbContext productDbContext) : base(productDbContext)
        {
        }
        public async Task<(IEnumerable<Products> Product, int TotalCount)> GetAllProductsAsync(bool includePromotion)
        {
            List<Products> products = new List<Products>();
            // Fetch data and map to ProductDto
            IQueryable<Products> productsQuery = _dbContext.Products.AsQueryable();
            if (includePromotion)
            {
                // Include related promotions
                products = await productsQuery
               .Include(x => x.ProductPromotions)
               .ThenInclude(p => p.Promotion)
               .ToListAsync();
                //ProductList = _mapper.Map<List<ProductDto>>(productsWithPromotions);

            }
            else
            {
                products = await productsQuery.ToListAsync();
                // Map the fetched products (without promotions) to ProductDto using AutoMapper
                //ProductList = _mapper.Map<List<ProductDto>>(products);
            }

            int totalCount = products.Count();


            return (products, totalCount);
        }
        public async Task<Products> GetProductByIdAsync(long id, bool includePromotion)
        {
            Products Product = new();

            // Fetch data and map to ProductDto
            IQueryable<Products> productsQuery = _dbContext.Products.AsQueryable();
            if (includePromotion)
            {
                // Include related promotions
                List<Products> productsWithPromotions = await productsQuery
               .Include(x => x.ProductPromotions)
               .ThenInclude(p => p.Promotion)
               .ToListAsync();
                //  Product = _mapper.Map<ProductDto>(productsWithPromotions);
            }
            else
            {
                List<Products> products = await productsQuery.ToListAsync();
                // Map the fetched products (without promotions) to ProductDto using AutoMapper
                //  Product = _mapper.Map<ProductDto>(products);
            }

            return Product;
        }
    }
}
