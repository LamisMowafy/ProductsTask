using Infrastructure.Common;
using MediatR;

namespace Application.Services.Promotion.Command.Create
{
    public class CreatePromotion : IRequest<ServiceResponse<long>>
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public string Code { get; set; }  
    }
}
