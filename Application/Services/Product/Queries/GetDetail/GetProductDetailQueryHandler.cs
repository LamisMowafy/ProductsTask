using AutoMapper;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.Product.Queries.GetDetail
{
    public class GetProductDetailQueryHandler : IRequestHandler<GetProductDetailQuery, ProductDetailDto>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IMapper _mapper;

        public GetProductDetailQueryHandler(IProductRepository ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }
        public async Task<ProductDetailDto> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            Domain.Models.Products Product = await _ProductRepository.GetProductByIdAsync(request.ProductId, true);

            return _mapper.Map<ProductDetailDto>(Product);
        }
    }
}
