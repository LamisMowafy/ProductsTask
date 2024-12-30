using Domain.DTOs.Product;
using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Queries.GetList
{
    public  class GetProductsListQuery : IRequest<List<ProductDto>>
    {
        public SearchCriteria request { get; set; }
    }
}
