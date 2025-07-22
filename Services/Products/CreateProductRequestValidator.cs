using FluentValidation;

namespace App.Services.Products;

public class CreateProductRequestValidator:AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Product name is required.")
            .NotEmpty().WithMessage("Product name cannot be empty.")
            .Length(5, 250).WithMessage("Product name must be between 5 and 250 characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative.");




    }
}

