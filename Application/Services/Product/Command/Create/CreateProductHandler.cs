using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Resource;
using System.Net;
using System.Security.Claims;
namespace Application.Services.Product.Command.Create
{
    public class CreateProductHandler : IRequestHandler<CreateProduct, ServiceResponse<long>>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _Productcreated = "SUCCESSMESSAGE_PRODUCT_CREATED";
        private const string _notvalid = "NOT_VALID";
        public CreateProductHandler(IProductRepository ProductRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<long>> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<long>(0, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Products Product = _mapper.Map<Products>(request);
                CreateValidator validator = new();
                FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(request);
                if (result.Errors.Any())
                {
                    return new ServiceResponse<long>(0, _resourceHelper.Shared(_notvalid) ?? "", HttpStatusCode.Forbidden, ResponseStatus.FAILURE);
                }
                Product.CreatedBy = 1;
                Product.CreatedOn = DateTime.Now;
                Product = await _ProductRepository.AddAsync(Product);
                return new ServiceResponse<long>(Product.Id, _resourceHelper.Product(_Productcreated) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<long>(0, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
