using FluentValidation;
using HRTaskManagement.Application.DTOs.LeaveRequest;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Application.Validators
{
    public class CreateLeaveRequestDtoValidator : AbstractValidator<CreateLeaveRequestDto>
    {
        public CreateLeaveRequestDtoValidator()
        {
            RuleFor(x => x.LeaveType)
                .NotEmpty().WithMessage("İzin türü zorunludur.")
                .Must(s => Enum.TryParse<LeaveType>(s, true, out _))
                .WithMessage("Geçersiz izin türü. Geçerli değerler: Annual, Sick, Unpaid, Maternity, Other.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Başlangıç tarihi geçmişte olamaz.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitiş tarihi, başlangıç tarihinden önce olamaz.");

            RuleFor(x => x.Reason).MaximumLength(500);
        }
    }
}
