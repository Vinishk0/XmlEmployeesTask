namespace TestTask.Models
{
    public sealed class PaymentInput
    {
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string Amount { get; init; }
        public required string Month { get; init; }
    }
}
