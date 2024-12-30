using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.Product.Command.Delete
{
    public class DeleteProductHandler : IRequestHandler<DeleteProduct, Unit>
    {
        private readonly IProductRepository _ProductRepository;
        public DeleteProductHandler(IProductRepository ProductRepository)
        {
            _ProductRepository = ProductRepository;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            Domain.Models.Products Product = await _ProductRepository.GetByIdAsync(request.ProductId);

            await _ProductRepository.DeleteAsync(Product);

            return Unit.Value;
        }
    }
}
