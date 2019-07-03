using System;
using log4net.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace log4net.Extensions.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            createService().GetService<Class1>().Run();
        }
        public static IServiceProvider createService()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddScoped<AppLogger, AppLogger>()
                .AddScoped<Class1, Class1>().BuildServiceProvider();
            serviceProvider.GetService<ILoggerFactory>()
                .AddLog4Net(10,"myapp",AppDomain.CurrentDomain.BaseDirectory + "Config\\log4net.config",false);
            return serviceProvider;
        }
    }
}
