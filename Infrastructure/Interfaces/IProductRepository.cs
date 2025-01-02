using Domain.Models;
using Infrastructure.Common;

namespace Infrastructure.Interfaces
{
    public interface IProductRepository : IRepository<Products>
    {
        Task<PaginatedList<Products>> GetAllProductsAsync(int PageNumber, int PageSize, string SearchText, decimal? MinPrice, decimal? MaxPrice, bool IsFeatured, bool IsNew, bool includePromotion);
        Task<Products> GetProductByIdAsync(long id, bool includePromotion);
    }
}
