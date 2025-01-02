using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Product.Queries.GetList;
using Domain.DTOs.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductTask.Controllers.admin
{
    [Route("[controller]/admin")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminProductController(IMediator mediator)
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
      
        [HttpPost(Name = "AddProduct")]
        public async Task<ActionResult<long>> Create([FromBody] CreateProduct createCommand)
        {
            return Ok(await _mediator.Send(createCommand));
        }

        [HttpPut(Name = "UpdateProduct")]
        public async Task<ActionResult> Update([FromBody] UpdateProduct updateCommand)
        { 
            return Ok(await _mediator.Send(updateCommand));
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<ActionResult> Delete(long id)
        {
            DeleteProduct deleteCommand = new() { ProductId = id }; 
            return Ok(await _mediator.Send(deleteCommand));
        }
    }
}
