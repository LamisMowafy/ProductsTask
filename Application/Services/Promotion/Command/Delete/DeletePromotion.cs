using Infrastructure.Common;
using MediatR;

namespace Application.Services.Promotion.Command.Delete
{
    public class DeletePromotion : IRequest<ServiceResponse<Unit>>
    {
        public long PromotionId { get; set; }
    }
}
