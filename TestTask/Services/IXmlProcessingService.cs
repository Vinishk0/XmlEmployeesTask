using System.Collections.Generic;
using TestTask.Models;

namespace TestTask.Services
{
    public interface IXmlProcessingService
    {
        void ProcessTransformation(string dataPath, string xsltPath, string employeesPath);
        void AddNewPayment(string dataPath, string name, string surname, string amountStr, string month);
        List<EmployeeSummary> GetEmployeeSummaries(string employeesPath);
        List<MonthlySummary> GetMonthlySummaries(string dataPath);
    }
}