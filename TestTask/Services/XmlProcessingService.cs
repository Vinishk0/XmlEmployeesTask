using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Xsl;
using TestTask.Constants;
using TestTask.Models;
using TestTask.Utils;

namespace TestTask.Services
{
    public sealed class XmlProcessingService : IXmlProcessingService
    {
        private readonly IXmlTotalsUpdater _totalsUpdater;

        public XmlProcessingService(IXmlTotalsUpdater totalsUpdater)
        {
            _totalsUpdater = totalsUpdater;
        }

        public void ProcessTransformation(string dataPath, string xsltPath, string employeesPath)
        {
            EnsureFileExists(dataPath, "Файл данных (.xml) не найден.");
            EnsureFileExists(xsltPath, "Transform.xslt не найден.");

            XslCompiledTransform xslt = new();
            xslt.Load(xsltPath);
            xslt.Transform(dataPath, employeesPath);

            _totalsUpdater.UpdateEmployeesWithTotal(employeesPath);
            _totalsUpdater.UpdateSourceFileWithTotal(dataPath);
        }

        public void AddNewPayment(string dataPath, PaymentInput payment)
        {
            EnsureFileExists(dataPath, "Файл данных не найден.");

            XDocument doc = XDocument.Load(dataPath);

            XElement newItem = new(XmlNames.Item,
                new XAttribute(XmlNames.Name, payment.Name),
                new XAttribute(XmlNames.Surname, payment.Surname),
                new XAttribute(XmlNames.Amount, payment.Amount),
                new XAttribute(XmlNames.Mount, payment.Month));

            doc.Root?.Add(newItem);
            doc.Save(dataPath);
        }

        public IReadOnlyList<EmployeeSummary> GetEmployeeSummaries(string employeesPath)
        {
            if (!File.Exists(employeesPath))
                return [];

            XDocument empDoc = XDocument.Load(employeesPath);

            return empDoc.Descendants(XmlNames.Employee)
                .Select(e => new EmployeeSummary
                {
                    Name = e.Attribute(XmlNames.Name)?.Value ?? string.Empty,
                    Surname = e.Attribute(XmlNames.Surname)?.Value ?? string.Empty,
                    TotalSalary = AmountParser.ParseOrDefault(e.Attribute(XmlNames.TotalSalary)?.Value)
                })
                .OrderBy(e => e.Surname)
                .ThenBy(e => e.Name)
                .ToList();
        }

        public IReadOnlyList<MonthlySummary> GetMonthlySummaries(string dataPath)
        {
            if (!File.Exists(dataPath))
                return [];

            XDocument dataDoc = XDocument.Load(dataPath);

            return dataDoc.Descendants(XmlNames.Item)
                .GroupBy(i => i.Attribute(XmlNames.Mount)?.Value)
                .Select(g => new MonthlySummary
                {
                    MonthKey = g.Key ?? string.Empty,
                    Month = MonthHelper.GetMonthRu(g.Key),
                    TotalAmount = g.Sum(i => AmountParser.ParseOrDefault(i.Attribute(XmlNames.Amount)?.Value))
                })
                .OrderBy(m => MonthHelper.GetMonthOrder(m.MonthKey))
                .ToList();
        }

        private static void EnsureFileExists(string path, string message)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException(message);
        }
    }
}
