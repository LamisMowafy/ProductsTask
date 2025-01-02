using FluentValidation;

namespace Application.Services.Promotion.Command.Update
{
    public class UpdateValidator : AbstractValidator<UpdatePromotion>
    {
        public UpdateValidator()
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
