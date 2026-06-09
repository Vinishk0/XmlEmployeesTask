namespace TestTask.Services
{
    public interface IXmlTotalsUpdater
    {
        void UpdateEmployeesWithTotal(string employeesPath);
        void UpdateSourceFileWithTotal(string dataPath);
    }
}
