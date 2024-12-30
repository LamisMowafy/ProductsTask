using MediatR;

namespace Application.Services.Product.Command.Delete
{
    public class DeleteProduct : IRequest<Unit>
    {
        public long ProductId { get; set; }
    }

}
