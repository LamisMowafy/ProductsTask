using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Queries.GetDetail
{
    public class GetProductDetailQuery : IRequest<ServiceResponse<ProductDetailDto>>
    {
        public long ProductId { get; set; }
    }
}
