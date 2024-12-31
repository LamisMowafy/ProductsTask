using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Product.Queries.GetList;
using Domain.DTOs.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ProductTask.Controllers
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

        [HttpGet("all", Name = "GetAllProducts")]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            List<ProductDto> dtos = await _mediator.Send(new GetProductsListQuery());
            return Ok(dtos);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductDto>> GetProductById(long id)
        {
            GetProductDetailQuery getDetailQuery = new() { ProductId = id };
            return Ok(await _mediator.Send(getDetailQuery));
        }
        [Authorize(Policy = "Admin")]
        [HttpPost(Name = "AddProduct")]
        public async Task<ActionResult<long>> Create([FromBody] CreateProduct createCommand)
        {
            long id = await _mediator.Send(createCommand);
            return Ok(id);
        }

        [HttpPut(Name = "UpdateProduct")]
        public async Task<ActionResult> Update([FromBody] UpdateProduct updateCommand)
        {
            await _mediator.Send(updateCommand);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<ActionResult> Delete(long id)
        {
            DeleteProduct deleteCommand = new() { ProductId = id };
            await _mediator.Send(deleteCommand);
            return NoContent();
        }
    }
}
