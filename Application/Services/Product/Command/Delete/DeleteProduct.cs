using Infrastructure.Common;
using MediatR;

namespace Application.Services.Product.Command.Delete
{
    public class DeleteProduct : IRequest<ServiceResponse<Unit>>
    {
        public long ProductId { get; set; }
    }

}
