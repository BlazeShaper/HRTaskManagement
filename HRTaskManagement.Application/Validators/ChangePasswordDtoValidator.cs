using FluentValidation;
using HRTaskManagement.Application.DTOs.Auth;

namespace HRTaskManagement.Application.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Yeni şifre en az bir büyük harf içermeli.")
                .Matches("[a-z]").WithMessage("Yeni şifre en az bir küçük harf içermeli.")
                .Matches("[0-9]").WithMessage("Yeni şifre en az bir rakam içermeli.")
                .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre eskisiyle aynı olamaz.");
        }
    }
}