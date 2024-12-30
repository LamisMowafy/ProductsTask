using MediatR;

namespace Application.Services.Product.Command.Create
{
    public class CreateProduct : IRequest<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } 

    }
}
