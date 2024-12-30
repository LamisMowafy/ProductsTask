using Domain.DTOs.Product;
using MediatR;

namespace Application.Services.Product.Queries.GetDetail
{
    public class GetProductDetailQuery : IRequest<ProductDetailDto>
    {
        public long ProductId { get; set; }
    }
}
