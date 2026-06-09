using System;
using TestTask.Models;
using TestTask.Utils;

namespace TestTask.Services
{
    public sealed class PaymentValidator : IPaymentValidator
    {
        public ValidationResult Validate(PaymentInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name) ||
                string.IsNullOrWhiteSpace(input.Surname) ||
                string.IsNullOrWhiteSpace(input.Amount) ||
                string.IsNullOrWhiteSpace(input.Month))
            {
                return ValidationResult.Failure("Пожалуйста, заполните все поля.");
            }

            if (!AmountParser.TryParse(input.Amount, out _))
            {
                return ValidationResult.Failure("Недопустимый формат суммы.");
            }

            return ValidationResult.Success();
        }
    }
}
