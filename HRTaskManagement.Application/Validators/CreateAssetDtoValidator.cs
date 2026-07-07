using FluentValidation;
using HRTaskManagement.Application.DTOs.Asset;

namespace HRTaskManagement.Application.Validators
{
    public class CreateAssetDtoValidator : AbstractValidator<CreateAssetDto>
    {
        public CreateAssetDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Demirbaş adı zorunludur.")
                .MaximumLength(150);

            RuleFor(x => x.AssetType)
                .NotEmpty().WithMessage("Demirbaş tipi zorunludur.")
                .MaximumLength(100);

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Seri numarası zorunludur.")
                .MaximumLength(100);

            RuleFor(x => x.PurchaseDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Satın alma tarihi gelecekte olamaz.");
        }
    }
}
