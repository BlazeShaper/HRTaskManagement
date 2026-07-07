using FluentValidation;
using HRTaskManagement.Application.DTOs.Asset;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Application.Validators
{
    public class UpdateAssetDtoValidator : AbstractValidator<UpdateAssetDto>
    {
        public UpdateAssetDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Demirbaş adı zorunludur.")
                .MaximumLength(150);

            RuleFor(x => x.AssetType)
                .NotEmpty().WithMessage("Demirbaş tipi zorunludur.")
                .MaximumLength(100);

            RuleFor(x => x.PurchaseDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Satın alma tarihi gelecekte olamaz.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(s => Enum.TryParse<AssetStatus>(s, true, out _))
                .WithMessage("Geçersiz durum değeri. Geçerli değerler: Available, Assigned, UnderRepair, Retired.");
        }
    }
}
