using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace log4net.Extension
{
    public static class Log4netExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory,int applicationId, string applicationName, string path, bool skipDiagnosticLogs)
        {
            factory.AddProvider(new Log4NetProvider(applicationId, applicationName, path, skipDiagnosticLogs));
            return factory;
        }
    }
}
