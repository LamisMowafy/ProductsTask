using Application.Services.Product.Command.Create;
using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.PromotionProduct.create
{ 
    public class PromotionToProductCommandHandler : IRequestHandler<PromotionToProduct, long>
{
    //private readonly IProductRepository _ProductRepository;
    //private readonly IMapper _mapper;

    //public PromotionToProductCommandHandler(IProductRepository ProductRepository, IMapper mapper)
    //{
    //    _ProductRepository = ProductRepository;
    //    _mapper = mapper;
    //}
    //public async Task<long> Handle(CreateProduct request, CancellationToken cancellationToken)
    //{
    //    PromotionToProduct Product = _mapper.Map<PromotionToProduct>(request);
    //    Product = await _ProductRepository.AddAsync(Product);
    //    return Product.Id;
    //}
}
}
