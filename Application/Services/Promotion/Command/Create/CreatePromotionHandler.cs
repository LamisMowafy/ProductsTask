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
namespace Application.Services.Promotion.Command.Create
{
    public class CreatePromotionHandler : IRequestHandler<CreatePromotion, ServiceResponse<long>>
    {
        private readonly IPromotionRepository _PromotionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _Promotioncreated = "SUCCESSMESSAGE_PROMOTION_CREATED";
        private const string _notvalid = "NOT_VALID";
        public CreatePromotionHandler(IPromotionRepository PromotionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _PromotionRepository = PromotionRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<long>> Handle(CreatePromotion request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<long>(0, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Promotions Promotion = _mapper.Map<Promotions>(request);
                CreateValidator validator = new();
                FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(request);
                if (result.Errors.Any())
                {
                    return new ServiceResponse<long>(0, _resourceHelper.Shared(_notvalid) ?? "", HttpStatusCode.Forbidden, ResponseStatus.FAILURE);
                }
                Promotion.CreatedBy = 1;
                Promotion.CreatedOn = DateTime.Now;
                Promotion = await _PromotionRepository.AddAsync(Promotion);
                return new ServiceResponse<long>(Promotion.Id, _resourceHelper.Promotion(_Promotioncreated) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<long>(0, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
