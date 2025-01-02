using Azure.Core;
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
        public async Task<Products> GetProductByIdAsync(long id, bool includePromotion)
        {
            // Fetch data and map to ProductDto
            IQueryable<Products> productsQuery = _dbContext.Products.Where(n => n.Id == id).AsQueryable();
            if (includePromotion)
            {
                productsQuery = productsQuery
               .Include(x => x.ProductPromotions)
               .ThenInclude(p => p.Promotion);
            }
            return await productsQuery.FirstOrDefaultAsync();
        }
        public async Task<PaginatedList<Products>> GetAllProductsAsync(int pageNumber, int pageSize, string SearchText, decimal? minPrice, decimal? maxPrice,bool IsFeatured,bool IsNew, bool includePromotion)
        {
            IQueryable<Products> query = _dbContext.Products.AsQueryable();
            if (includePromotion)
            {
                // Include related promotions
                query = query.Include(x => x.ProductPromotions).ThenInclude(p => p.Promotion);
                if (IsFeatured)
                {
                    query = query.Where(p => p.ProductPromotions.Select(h=>h.ProductId).Contains(p.Id));// if  Product Has  Promotion Then Is Featured
                }
            }
            // Apply filtering and searching
            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(p => p.Name.Contains(SearchText) || p.Description.Contains(SearchText));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }
            if (IsNew)
            {
                query = query.Where(p => p.NewedUntil <= DateTime.Now.Date);
            }
            // Paginate the result
            return await PaginatedList<Products>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
