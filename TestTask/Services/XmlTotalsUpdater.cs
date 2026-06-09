using System.IO;
using System.Linq;
using System.Xml.Linq;
using TestTask.Constants;
using TestTask.Utils;

namespace TestTask.Services
{
    public sealed class XmlTotalsUpdater : IXmlTotalsUpdater
    {
        public void UpdateEmployeesWithTotal(string employeesPath)
        {
            if (!File.Exists(employeesPath))
                return;

            XDocument doc = XDocument.Load(employeesPath);

            foreach (XElement employee in doc.Descendants(XmlNames.Employee))
            {
                double total = employee.Elements(XmlNames.Salary)
                    .Select(s => AmountParser.ParseOrDefault(s.Attribute(XmlNames.Amount)?.Value))
                    .Sum();

                employee.SetAttributeValue(XmlNames.TotalSalary, AmountParser.Format(total));
            }

            doc.Save(employeesPath);
        }

        public void UpdateSourceFileWithTotal(string dataPath)
        {
            if (!File.Exists(dataPath))
                return;

            XDocument doc = XDocument.Load(dataPath);

            double total = doc.Descendants(XmlNames.Item)
                .Select(i => AmountParser.ParseOrDefault(i.Attribute(XmlNames.Amount)?.Value))
                .Sum();

            doc.Root?.SetAttributeValue(XmlNames.TotalAmount, AmountParser.Format(total));
            doc.Save(dataPath);
        }
    }
}
