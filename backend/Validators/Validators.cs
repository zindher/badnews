using FluentValidation;
using BadNews.DTOs;

namespace BadNews.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letters")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letters")
            .Matches(@"[0-9]").WithMessage("Password must contain numbers");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is invalid");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.RecipientPhone)
            .NotEmpty().WithMessage("Recipient phone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is invalid");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters")
            .Must(BeValidWordCount).WithMessage("Message cannot exceed 250 words");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10000 pesos");

        RuleFor(x => x.MessageType)
            .NotEmpty().WithMessage("Message type is required")
            .Must(x => x.ToLower() == "joke" || x.ToLower() == "confession" || x.ToLower() == "truth")
            .WithMessage("Message type must be joke, confession, or truth");
    }

    private bool BeValidWordCount(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Contar palabras: dividir por espacios y caracteres especiales
        var words = message.Trim().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length <= 250;
    }
}

public class RateOrderDtoValidator : AbstractValidator<RateOrderDto>
{
    public RateOrderDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Review)
            .MaximumLength(500).WithMessage("Review cannot exceed 500 characters");
    }
}

public class UpdateMessengerAvailabilityDtoValidator : AbstractValidator<UpdateMessengerAvailabilityDto>
{
    public UpdateMessengerAvailabilityDtoValidator()
    {
        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Availability status is required");

        RuleFor(x => x.MaxCallsPerDay)
            .GreaterThan(0).When(x => x.IsAvailable)
            .WithMessage("Max calls per day must be greater than 0 when available")
            .LessThanOrEqualTo(50).WithMessage("Max calls per day cannot exceed 50");
    }
}

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("Order ID must be valid");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("Payment method is required");
    }
}

public class WithdrawFundsDtoValidator : AbstractValidator<WithdrawFundsDto>
{
    public WithdrawFundsDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Withdrawal amount must be greater than 0");

        RuleFor(x => x.BankAccountNumber)
            .NotEmpty().WithMessage("Bank account number is required")
            .MaximumLength(20).WithMessage("Bank account number cannot exceed 20 characters");

        RuleFor(x => x.BankCode)
            .NotEmpty().WithMessage("Bank code is required");
    }
}
