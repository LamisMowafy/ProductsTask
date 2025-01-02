using FluentValidation;

namespace Application.Services.Promotion.Command.Create
{
    public class CreateValidator : AbstractValidator<CreatePromotion>
    {
        public CreateValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(200);
            RuleFor(p => p.Description)
                .NotEmpty()
                .NotNull();
            RuleFor(p => p.StartDate)
               .NotEmpty()
               .NotNull();
        }
    }
}
