using FluentValidation;
using HRTaskManagement.Application.DTOs.Position;

namespace HRTaskManagement.Application.Validators
{
    public class UpdatePositionDtoValidator : AbstractValidator<UpdatePositionDto>
    {
        public UpdatePositionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Pozisyon adı zorunludur.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}