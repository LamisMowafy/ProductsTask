using AutoMapper;
using Domain.Enums;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Resource;
using System.Net;

namespace Application.Services.Product.Queries.GetDetail
{
    public class GetProductDetailQueryHandler : IRequestHandler<GetProductDetailQuery, ServiceResponse<ProductDetailDto>>
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _dataRetrievedSuccessFully = "SUCCESSMESSAGE_DATA_RETIRVED_SUCCESSFULLY";
        private const string _internalError = "ERROREMESSAGE_INTERNAL"; 
        private const string _ProductNotExist = "ERRORMESSAGE_PRODUCT_NOT_EXIST";
        #endregion
        public GetProductDetailQueryHandler(IProductRepository ProductRepository, IMapper mapper, IResourceHelper resourceHelper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<ProductDetailDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Domain.Models.Products Product = await _ProductRepository.GetProductByIdAsync(request.ProductId, true);
                if (Product == null)
                {
                    return new ServiceResponse<ProductDetailDto>(new ProductDetailDto(), _resourceHelper.Product(_ProductNotExist) ?? "", HttpStatusCode.NotFound, ResponseStatus.NOT_FOUND);
                }
                return new ServiceResponse<ProductDetailDto>(_mapper.Map<ProductDetailDto>(Product), _resourceHelper.Product(_dataRetrievedSuccessFully) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<ProductDetailDto>(new ProductDetailDto(), _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
