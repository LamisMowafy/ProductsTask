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

namespace Application.Services.ProductPromotion.Command.Create
{
    public class CreateProductPromotionHandler : IRequestHandler<CreateProductPromotion, ServiceResponse<long>>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IPromotionRepository _PromotionRepository;
        private readonly IProductPromotionRepository _ProductPromotionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _RoleRequired = "ROLE_REQUIRED";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        private const string _productpromotioncreated = "SUCCESSMESSAGE_PRODUCTPROMOTION_CREATED";
        private const string _ProductNotExist = "ERRORMESSAGE_PRODUCT_NOT_EXIST";
        private const string _promotionNotExist = "ERRORMESSAGE_PROMOTION_NOT_EXIST";
        private const string _productassociated = "PRODUCT_ASSOCIATED";
        #endregion
        public CreateProductPromotionHandler(IProductRepository ProductRepository, IPromotionRepository PromotionRepository,
            IProductPromotionRepository ProductPromotionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IResourceHelper resourceHelper)
        {
            _ProductRepository = ProductRepository;
            _PromotionRepository = PromotionRepository;
            _ProductPromotionRepository = ProductPromotionRepository;

            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<long>> Handle(CreateProductPromotion request, CancellationToken cancellationToken)
        {
            try
            {
                ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;
                if (userClaims == null || !userClaims.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    return new ServiceResponse<long>(0, _resourceHelper.User(_RoleRequired) ?? "", HttpStatusCode.Unauthorized, ResponseStatus.NOT_ALLOWED);
                }
                Domain.Models.Products Product = await _ProductRepository.GetByIdAsync(request.ProductId);
                if (Product == null)
                {
                    return new ServiceResponse<long>(0, _resourceHelper.Product(_ProductNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                }
                Domain.Models.Promotions Promotion = await _PromotionRepository.GetByIdAsync(request.PromotionId);
                if (Promotion == null)
                {
                    return new ServiceResponse<long>(0, _resourceHelper.Promotion(_promotionNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                }
                bool Exist = await _ProductPromotionRepository.CheckIfProductPromotionIsExist(request.ProductId ,request.PromotionId);
                if (Exist)
                {
                    return new ServiceResponse<long>(0, _resourceHelper.Promotion(_productassociated) ?? "", HttpStatusCode.Conflict, ResponseStatus.FAILURE);
                }
                Domain.Models.ProductPromotion ProductPromotion = _mapper.Map<Domain.Models.ProductPromotion>(request);
                ProductPromotion.CreatedBy = 1;
                ProductPromotion.CreatedOn = DateTime.Now;
                ProductPromotion = await _ProductPromotionRepository.AddAsync(ProductPromotion);
                return new ServiceResponse<long>(ProductPromotion.Id, _resourceHelper.Product(_productpromotioncreated) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<long>(0, _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
