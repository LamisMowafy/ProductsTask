using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Product.Queries.GetList;
using Domain.DTOs.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ProductTask.Controllers.Client
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts([FromQuery] GetProductsListQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(long id)
        {
            GetProductDetailQuery getDetailQuery = new() { ProductId = id };
            return Ok(await _mediator.Send(getDetailQuery));
        }
 
    }
}
