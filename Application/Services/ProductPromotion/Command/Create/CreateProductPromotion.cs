using Infrastructure.Common;
using MediatR;

namespace Application.Services.ProductPromotion.Command.Create
{
    public class CreateProductPromotion : IRequest<ServiceResponse<long>>
    {
        public long ProductId { get; set; }
        public long PromotionId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
