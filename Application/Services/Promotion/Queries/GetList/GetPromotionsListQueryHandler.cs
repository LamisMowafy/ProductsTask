using AutoMapper;
using Domain.DTOs.Promotion;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using Resource;
using System.Net;

namespace Application.Services.Promotion.Queries.GetList
{
    public class GetPromotionsListQueryHandler
    {
        private readonly IPromotionRepository _PromotionRepository;
        private readonly IMapper _mapper;
        private readonly IResourceHelper _resourceHelper;
        private const string _dataRetrievedSuccessFully = "SUCCESSMESSAGE_DATA_RETIRVED_SUCCESSFULLY";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";

        public GetPromotionsListQueryHandler(IPromotionRepository PromotionRepository, IMapper mapper, IResourceHelper resourceHelper)
        {
            _PromotionRepository = PromotionRepository;
            _mapper = mapper;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<List<PromotionDto>>> Handle(GetPromotionsListQuery request, CancellationToken cancellationToken)
        {
            try
            { 
                PaginatedList<Promotions> allPromotions = (PaginatedList<Promotions>)await _PromotionRepository.ListAllAsync();
                return new ServiceResponse<List<PromotionDto>>(_mapper.Map<List<PromotionDto>>(allPromotions), _resourceHelper.Promotion(_dataRetrievedSuccessFully) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<List<PromotionDto>>([], _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
