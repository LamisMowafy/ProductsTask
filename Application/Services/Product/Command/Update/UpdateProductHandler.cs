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

namespace Application.Services.Product.Command.Update
{
    public class UpdateProductHandler : IRequestHandler<UpdateProduct, ServiceResponse<Unit>>
    {
        private readonly IRepository<Products> _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _productupdated = "SUCCESSMESSAGE_PRODUCT_UPDATED";
        private const string _notvalid = "NOT_VALID";
        private const string _ProductNotExist = "ERRORMESSAGE_PRODUCT_NOT_EXIST";

        #endregion
        public UpdateProductHandler(IRepository<Products> ProductRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<Unit>> Handle(UpdateProduct request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                 
                UpdateValidator validator = new();
                FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(request);
                if (result.Errors.Any())
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_notvalid) ?? "", HttpStatusCode.Forbidden, ResponseStatus.FAILURE);
                }
                Domain.Models.Products Product = await _ProductRepository.GetByIdAsync(request.Id);
                if (Product == null)
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_ProductNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                }
                Product.ModifiedBy =1;
                Product.ModifiedOn = DateTime.Now;
                await _ProductRepository.UpdateAsync(Product);
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_productupdated) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }

        }


    }
}
