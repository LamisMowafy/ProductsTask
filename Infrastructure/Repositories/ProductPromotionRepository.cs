using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class ProductPromotionRepository : Repository<ProductPromotion>, IProductPromotionRepository
    {
        public ProductPromotionRepository(ProductDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> CheckIfProductPromotionIsExist(long ProductId, long PromotionId)
        {
            bool IsExist = await _dbContext.ProductPromotion
                .AnyAsync(n => n.ProductId == ProductId && n.PromotionId == PromotionId);

            return IsExist;
        }
    }
}
