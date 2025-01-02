using Domain.DTOs.Product;
using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Queries.GetList
{
    public class GetProductsListQuery : IRequest<ServiceResponse<List<ProductDto>>>
    {
        public SearchCriteria? SearchCriteria { get; set; }
        public decimal? MinPrice { get; set; } // Filter for minimum price
        public decimal? MaxPrice { get; set; } // Filter for maximum price 
        public bool IsFeatured { get; set; } = false;  // Whether the product is featured
        public bool IsNew { get; set; } = false;  // Whether the product is new 


    }
}
