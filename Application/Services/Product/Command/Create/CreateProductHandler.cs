using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.Product.Command.Create
{
    public class CreateProductHandler : IRequestHandler<CreateProduct, long>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateProductHandler(IProductRepository ProductRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<long> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;

            if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                throw new UnauthorizedAccessException("User does not have the required role.");
            }

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
