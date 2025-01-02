using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Resource;
using System.Net;
using System.Security.Claims;

namespace Application.Services.Product.Command.Delete
{
    public class DeleteProductHandler : IRequestHandler<DeleteProduct, ServiceResponse<Unit>>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _productdeleted = "SUCCESSMESSAGE_PRODUCT_DELETED";
        private const string _ProductNotExist = "ERRORMESSAGE_PRODUCT_NOT_EXIST";
        #endregion
        public DeleteProductHandler(IProductRepository ProductRepository, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _ProductRepository = ProductRepository;
            _resourceHelper = resourceHelper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<Unit>> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Domain.Models.Products Product = await _ProductRepository.GetByIdAsync(request.ProductId);
                if (Product == null)
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_ProductNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                } 
                await _ProductRepository.DeleteAsync(Product);
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_productdeleted) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
