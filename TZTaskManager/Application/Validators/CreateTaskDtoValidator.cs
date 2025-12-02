using FluentValidation;
using TZTaskManager.Application.DTOs;

namespace TZTaskManager.Application.Validators
{
    /// <summary>
    /// Валидатор для CreateTaskDto
    /// </summary>
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название задачи обязательно")
                .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.TaskTypeId)
                .GreaterThan(0).WithMessage("Тип задачи обязателен");
        }
    }
}

