using System;
using TestTask.Configuration;
using TestTask.Services;

namespace TestTask.Infrastructure
{
    public sealed class AppServices
    {
        public AppServices()
        {
            Paths = new AppPaths(AppDomain.CurrentDomain.BaseDirectory);

            IXmlTotalsUpdater totalsUpdater = new XmlTotalsUpdater();
            XmlService = new XmlProcessingService(totalsUpdater);
            PaymentValidator = new PaymentValidator();
        }

        public AppPaths Paths { get; }
        public IXmlProcessingService XmlService { get; }
        public IPaymentValidator PaymentValidator { get; }
    }
}
