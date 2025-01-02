using AutoMapper;
using Azure.Core;
using Domain.DTOs.Product;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using MediatR;
using Resource;
using System.Net;

namespace Application.Services.Product.Queries.GetList
{
    public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, ServiceResponse<List<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IResourceHelper _resourceHelper;
        #region Declaration 
        private const string _dataRetrievedSuccessFully = "SUCCESSMESSAGE_DATA_RETIRVED_SUCCESSFULLY";
        private const string _internalError = "ERROREMESSAGE_INTERNAL";
        #endregion

        public GetProductsListQueryHandler(IProductRepository productRepository, IMapper mapper, IResourceHelper resourceHelper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _resourceHelper = resourceHelper;
        }
        public async Task<ServiceResponse<List<ProductDto>>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
        {
            try
            { 
                request.SearchCriteria ??= new SearchCriteria();
                PaginatedList<Products> allProducts = await _productRepository.GetAllProductsAsync(request.SearchCriteria.PageNumber, request.SearchCriteria.PageSize, request.SearchCriteria.SearchText, request.MinPrice, request.MaxPrice,request.IsFeatured, request.IsNew, true);
                return new ServiceResponse<List<ProductDto>>(_mapper.Map<List<ProductDto>>(allProducts), _resourceHelper.Product(_dataRetrievedSuccessFully) ?? "", HttpStatusCode.OK, ResponseStatus.SUCCESS);
            }
            catch (Exception)
            {
                return new ServiceResponse<List<ProductDto>>(new List<ProductDto>(), _resourceHelper.Shared(_internalError) ?? "", HttpStatusCode.InternalServerError, ResponseStatus.ERROR);
            }
        }
    }
}
