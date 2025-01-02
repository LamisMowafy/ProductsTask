using FluentValidation;
namespace Application.Services.Product.Command.Update
{
    public class UpdateValidator : AbstractValidator<UpdateProduct>
    {
        public UpdateValidator()
        {
            RuleFor(p => p.Name).NotEmpty().NotNull().MaximumLength(200);
            RuleFor(p => p.Description).NotEmpty().NotNull();
            RuleFor(p => p.Id).NotEmpty().NotNull().NotEqual(0);
        }
    }
}
