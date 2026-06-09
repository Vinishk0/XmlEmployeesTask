using System.IO;

namespace TestTask.Configuration
{
    public sealed class AppPaths
    {
        public AppPaths(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
            EmployeesFile = Path.Combine(baseDirectory, "Employees.xml");
            XsltFile = Path.Combine(baseDirectory, "Transform.xslt");
            DefaultDataFile = Path.Combine(baseDirectory, "Data1.xml");
        }

        public string BaseDirectory { get; }
        public string EmployeesFile { get; }
        public string XsltFile { get; }
        public string DefaultDataFile { get; }
    }
}
