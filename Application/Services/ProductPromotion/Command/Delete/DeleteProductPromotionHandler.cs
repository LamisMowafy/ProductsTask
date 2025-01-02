using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Resource;
using System.Net;
using System.Security.Claims;

namespace Application.Services.ProductPromotion.Command.Delete
{
    public class DeleteProductPromotionHandler : IRequestHandler<DeleteProductPromotion, ServiceResponse<Unit>>
    {
        private readonly IProductPromotionRepository _ProductPromotionRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _productPromotiondeleted = "SUCCESSMESSAGE_PRODUCTPROMOTION_DELETED";
        #endregion
        public DeleteProductPromotionHandler(IProductPromotionRepository ProductPromotionRepository, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _ProductPromotionRepository = ProductPromotionRepository;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<Unit>> Handle(DeleteProductPromotion request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
            if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
            }
            Domain.Models.ProductPromotion ProductPromotion = await _ProductPromotionRepository.GetByIdAsync(request.Id);

            await _ProductPromotionRepository.DeleteAsync(ProductPromotion);
            return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_productPromotiondeleted) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);

        }
    }
}
