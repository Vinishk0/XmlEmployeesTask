using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Xsl;
using TestTask.Utils;
using TestTask.Models;
using TestTask.Utils;

namespace TestTask.Services
{
    public class XmlProcessingService : IXmlProcessingService
    {
        public void ProcessTransformation(string dataPath, string xsltPath, string employeesPath)
        {
            if (string.IsNullOrWhiteSpace(dataPath) || !File.Exists(dataPath))
                throw new FileNotFoundException("Файл данных (.xml) не найден.");

            if (!File.Exists(xsltPath))
                throw new FileNotFoundException("Transform.xslt не найден.");

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltPath);
            xslt.Transform(dataPath, employeesPath);

            UpdateEmployeesWithTotal(employeesPath);
            UpdateSourceFileWithTotal(dataPath);
        }

        public void AddNewPayment(string dataPath, string name, string surname, string amountStr, string month)
        {
            if (!File.Exists(dataPath))
                throw new FileNotFoundException("Файл данных не найден.");

            XDocument doc = XDocument.Load(dataPath);

            XElement newItem = new XElement("item",
                new XAttribute("name", name),
                new XAttribute("surname", surname),
                new XAttribute("amount", amountStr),
                new XAttribute("mount", month)
            );

            doc.Root?.Add(newItem);
            doc.Save(dataPath);
        }

        public List<EmployeeSummary> GetEmployeeSummaries(string employeesPath)
        {
            if (!File.Exists(employeesPath))
                return new List<EmployeeSummary>();

            XDocument empDoc = XDocument.Load(employeesPath);

            return empDoc.Descendants("Employee")
                .Select(e => new EmployeeSummary
                {
                    Name = e.Attribute("name")?.Value,
                    Surname = e.Attribute("surname")?.Value,
                    TotalSalary = ParseAmount(e.Attribute("totalSalary")?.Value)
                })
                .OrderBy(e => e.Surname)
                .ThenBy(e => e.Name)
                .ToList();
        }

        public List<MonthlySummary> GetMonthlySummaries(string dataPath)
        {
            if (!File.Exists(dataPath))
                return new List<MonthlySummary>();

            XDocument dataDoc = XDocument.Load(dataPath);

            return dataDoc.Descendants("item")
                .GroupBy(i => i.Attribute("mount")?.Value)
                .Select(g => new MonthlySummary
                {
                    MonthKey = g.Key,
                    Month = MonthHelper.GetMonthRu(g.Key),
                    TotalAmount = g.Sum(i => ParseAmount(i.Attribute("amount")?.Value))
                })
                .OrderBy(m => MonthHelper.GetMonthOrder(m.MonthKey))
                .ToList();
        }

        private void UpdateEmployeesWithTotal(string employeesPath)
        {
            if (!File.Exists(employeesPath)) return;

            XDocument doc = XDocument.Load(employeesPath);

            foreach (var emp in doc.Descendants("Employee"))
            {
                double total = emp.Elements("salary")
                    .Select(s => ParseAmount(s.Attribute("amount")?.Value))
                    .Sum();

                emp.SetAttributeValue("totalSalary",
                    total.ToString("F2", CultureInfo.InvariantCulture));
            }

            doc.Save(employeesPath);
        }

        private void UpdateSourceFileWithTotal(string dataPath)
        {
            if (!File.Exists(dataPath)) return;

            XDocument doc = XDocument.Load(dataPath);

            double total = doc.Descendants("item")
                .Select(i => ParseAmount(i.Attribute("amount")?.Value))
                .Sum();

            if (doc.Root != null)
            {
                doc.Root.SetAttributeValue("totalAmount",
                    total.ToString("F2", CultureInfo.InvariantCulture));
            }

            doc.Save(dataPath);
        }

        private double ParseAmount(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;

            string normalized = value.Replace(',', '.');

            return double.TryParse(normalized,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double result)
                ? result : 0;
        }
    }
}