using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.Product.Command.Create
{
    public class CreateProductHandler : IRequestHandler<CreateProduct, long>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IMapper _mapper;

        public CreateProductHandler(IProductRepository ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }
        public async Task<long> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            Products Product = _mapper.Map<Products>(request);

            CreateValidator validator = new();
            FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(request);

            if (result.Errors.Any())
            {
                throw new Exception("Product is not valid");
            }

            Product = await _ProductRepository.AddAsync(Product);

            return Product.Id;
        }
    }
}
