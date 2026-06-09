using System.Collections.Generic;
using TestTask.Models;

namespace TestTask.Services
{
    public interface IXmlProcessingService
    {
        void ProcessTransformation(string dataPath, string xsltPath, string employeesPath);
        void AddNewPayment(string dataPath, PaymentInput payment);
        IReadOnlyList<EmployeeSummary> GetEmployeeSummaries(string employeesPath);
        IReadOnlyList<MonthlySummary> GetMonthlySummaries(string dataPath);
    }
}
