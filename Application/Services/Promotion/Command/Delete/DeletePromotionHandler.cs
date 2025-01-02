using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Resource;
using System.Net;
using System.Security.Claims;

namespace Application.Services.Promotion.Command.Delete
{
    public class DeletePromotionHandler : IRequestHandler<DeletePromotion, ServiceResponse<Unit>>
    {
        private readonly IPromotionRepository _PromotionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _promotiondeleted = "SUCCESSMESSAGE_PROMOTION_DELETED";
        private const string _promotionNotExist = "ERRORMESSAGE_PROMOTION_NOT_EXIST";
        public DeletePromotionHandler(IPromotionRepository PromotionRepository, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _PromotionRepository = PromotionRepository;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<Unit>> Handle(DeletePromotion request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Domain.Models.Promotions Promotion = await _PromotionRepository.GetByIdAsync(request.PromotionId);
                if (Promotion == null)
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Promotion(_promotionNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                }
                await _PromotionRepository.DeleteAsync(Promotion);
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Promotion(_promotiondeleted) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
