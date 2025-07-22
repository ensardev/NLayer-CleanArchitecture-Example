using FluentValidation;

namespace App.Services.Categories.Create;

public class CreateCategoryRequestValidator: AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Category name is required.")
            .Length(3, 150).WithMessage("Category name must be between 3 and 150 characters long.");
    }
}
