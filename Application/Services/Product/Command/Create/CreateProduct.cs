using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Command.Create
{
    public class CreateProduct : IRequest<ServiceResponse<long>>
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public DateTime? NewedUntil { get; set; }
    }
}
