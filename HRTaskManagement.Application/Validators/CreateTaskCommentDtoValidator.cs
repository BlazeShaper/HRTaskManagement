using FluentValidation;
using HRTaskManagement.Application.DTOs.TaskComment;

namespace HRTaskManagement.Application.Validators
{
    public class CreateTaskCommentDtoValidator : AbstractValidator<CreateTaskCommentDto>
    {
        public CreateTaskCommentDtoValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty();

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Yorum boş olamaz.")
                .MaximumLength(1000);
        }
    }
}
