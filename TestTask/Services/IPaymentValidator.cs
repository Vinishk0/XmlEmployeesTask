using TestTask.Models;

namespace TestTask.Services
{
    public interface IPaymentValidator
    {
        ValidationResult Validate(PaymentInput input);
    }

    public sealed class ValidationResult
    {
        public bool IsValid { get; init; }
        public string? ErrorMessage { get; init; }

        public static ValidationResult Success() => new() { IsValid = true };

        public static ValidationResult Failure(string message) =>
            new() { IsValid = false, ErrorMessage = message };
    }
}
