using FluentValidation;

namespace Application.Services.Product.Command.Create
{
    public class CreateValidator : AbstractValidator<CreateProduct>
    {
        public CreateValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(100);
            RuleFor(p => p.Description)
                .NotEmpty()
                .NotNull();
        }
    }

}
