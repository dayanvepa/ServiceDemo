using FluentValidation;
using ServiceDemo.Application.DTOs.Product;

namespace ServiceDemo.Application.Validators.Product
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        }
    }
}