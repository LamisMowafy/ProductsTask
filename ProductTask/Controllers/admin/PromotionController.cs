using Application.Services.Promotion.Command.Create;
using Application.Services.Promotion.Command.Delete;
using Application.Services.Promotion.Command.Update;
using Application.Services.Promotion.Queries.GetList;
using Domain.DTOs.Promotion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PromotionTask.Controllers.admin
{
    [Route("[controller]/admin")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class PromotionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PromotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllPromotions")]
        public async Task<ActionResult<List<PromotionDto>>> GetAllPromotions([FromQuery] GetPromotionsListQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

       
        [HttpPost(Name = "AddPromotion")]
        public async Task<ActionResult<long>> Create([FromBody] CreatePromotion createCommand)
        {
            return Ok(await _mediator.Send(createCommand));
        }

        [HttpPut(Name = "UpdatePromotion")]
        public async Task<ActionResult> Update([FromBody] UpdatePromotion updateCommand)
        {
            await _mediator.Send(updateCommand);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeletePromotion")]
        public async Task<ActionResult> Delete(long id)
        {
            DeletePromotion deleteCommand = new() { PromotionId = id };
            await _mediator.Send(deleteCommand);
            return NoContent();
        }
    }
}
