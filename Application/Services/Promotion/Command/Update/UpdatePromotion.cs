using Infrastructure.Common;
using MediatR;

namespace Application.Services.Promotion.Command.Update
{
    public class UpdatePromotion : IRequest<ServiceResponse<Unit>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public string Code { get; set; }
    }
}
