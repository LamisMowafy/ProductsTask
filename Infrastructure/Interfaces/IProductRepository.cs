using Domain.Models;

namespace Infrastructure.Interfaces
{
    public interface IProductRepository : IRepository<Products>
    {
        Task<(IEnumerable<Products> Product, int TotalCount)> GetAllProductsAsync(bool includePromotion);
        Task<Products> GetProductByIdAsync(long id, bool includePromotion);
    }
}
