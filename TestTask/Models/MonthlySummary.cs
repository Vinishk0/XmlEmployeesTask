namespace TestTask.Models
{
    public sealed class MonthlySummary
    {
        public string MonthKey { get; init; } = string.Empty;
        public string Month { get; init; } = string.Empty;
        public double TotalAmount { get; init; }
    }
}
