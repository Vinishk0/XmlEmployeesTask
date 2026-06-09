using System.Windows;
using TestTask.Infrastructure;

namespace TestTask
{
    public partial class App : Application
    {
        public static AppServices Services { get; } = new();
    }
}
