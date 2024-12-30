using AutoMapper;
using Domain.DTOs.Product;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.Product.Queries.GetList
{
    public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductsListQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<List<ProductDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
        {
            List<Products> ProductList = [];
          
            var allPosts = await _productRepository.GetAllProductsAsync( true);
            return _mapper.Map<List<ProductDto>>(allPosts);
        }
    }
}
