using Domain.Models;

namespace Infrastructure.Interfaces
{
    public interface IProductPromotionRepository : IRepository<ProductPromotion>
    {
        Task<bool> CheckIfProductPromotionIsExist(long ProductId, long PromotionId);
    }
}
