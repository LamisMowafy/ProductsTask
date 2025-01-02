using Infrastructure.Common;
using MediatR;

namespace Application.Services.ProductPromotion.Command.Delete
{
    public class DeleteProductPromotion : IRequest<ServiceResponse<Unit>>
    {
        public long Id { get; set; } 

    }
}
