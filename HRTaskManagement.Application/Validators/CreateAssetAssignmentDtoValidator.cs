using FluentValidation;
using HRTaskManagement.Application.DTOs.AssetAssignment;

namespace HRTaskManagement.Application.Validators
{
    public class CreateAssetAssignmentDtoValidator : AbstractValidator<CreateAssetAssignmentDto>
    {
        public CreateAssetAssignmentDtoValidator()
        {
            RuleFor(x => x.AssetId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.Note).MaximumLength(500);
        }
    }
}
