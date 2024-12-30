using AutoMapper;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Services.Product.Command.Update
{
    public class UpdateProductHandler : IRequestHandler<UpdateProduct, Unit>
    {
        private readonly IRepository<Products> _ProductRepository;
        private readonly IMapper _mapper;
        public UpdateProductHandler(IRepository<Products> ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateProduct request, CancellationToken cancellationToken)
        {
            Products Product = _mapper.Map<Products>(request);

            await _ProductRepository.UpdateAsync(Product);

            return Unit.Value;
        }


    }
}
