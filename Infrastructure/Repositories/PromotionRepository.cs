using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories
{
    public class PromotionRepository : Repository<Promotions>, IPromotionRepository
    {
        public PromotionRepository(ProductDbContext dbContext) : base(dbContext)
        {

        }
    }
}
