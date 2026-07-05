// Application/Validators/Employee/CreateEmployeeDtoValidator.cs
using System;
using FluentValidation;
using HRTaskManagement.Application.DTOs.Employee;

namespace HRTaskManagement.Application.Validators.Employee
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            // Email Kuralları
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email formatı giriniz.");

            // Telefon Kuralı
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon alanı boş olamaz.")
                .Matches(@"^\d{10}$").WithMessage("Telefon numarası 10 haneli olmalıdır.");

            // Yaş Kuralı (En Az 18)
            RuleFor(x => x.BirthDate)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("Çalışan en az 18 yaşında olmalıdır.");
        }

        private bool BeAtLeast18YearsOld(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;

            // Eğer doğum günü bu yıl henüz gelmediyse, yaşı 1 azalt
            if (birthDate > today.AddYears(-age))
                age--;

            return age >= 18;
        }
    }
}