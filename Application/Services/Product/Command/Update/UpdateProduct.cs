using MediatR;

namespace Application.Services.Product.Command.Update
{
    public class UpdateProduct : IRequest<Unit>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
