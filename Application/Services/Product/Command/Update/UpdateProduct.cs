using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Command.Update
{
    public class UpdateProduct : IRequest<ServiceResponse<Unit>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public DateTime? NewedUntil { get; set; }

    }
}
