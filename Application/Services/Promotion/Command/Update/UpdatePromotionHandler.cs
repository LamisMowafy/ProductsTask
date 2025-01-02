using Application.Services.Product.Command.Update;
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

namespace Application.Services.Promotion.Command.Update
{
    public class UpdatePromotionHandler : IRequestHandler<UpdatePromotion, ServiceResponse<Unit>>
    {
        private readonly IRepository<Promotions> _PromotionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _promotionupdated = "SUCCESSMESSAGE_PROMOTION_UPDATED";
        private const string _notvalid = "NOT_VALID";
        public UpdatePromotionHandler(IRepository<Promotions> PromotionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _PromotionRepository = PromotionRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<Unit>> Handle(UpdatePromotion request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Promotions Promotion = _mapper.Map<Promotions>(request);
                UpdateValidator validator = new();
                FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(request);
                if (result.Errors.Any())
                {
                    return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_notvalid) ?? "", HttpStatusCode.Forbidden, ResponseStatus.FAILURE);
                }
                Promotion.ModifiedBy = 1;
                Promotion.ModifiedOn = DateTime.Now;
                await _PromotionRepository.UpdateAsync(Promotion);
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Product(_promotionupdated) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<Unit>(Unit.Value, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }

        }


    }
}
