using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace log4net.Extension
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly int _applicationId;

        private readonly string _applicationName;

        private readonly string _log4NetConfigFile;

        private readonly bool _skipDiagnosticLogs;

        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new ConcurrentDictionary<string, ILogger>();

        public Log4NetProvider(int applicationId, string applicationName, string log4NetConfigFile, bool skipDiagnosticLogs)
        {
            _applicationId = applicationId;
            _applicationName = applicationName;
            _log4NetConfigFile = log4NetConfigFile;
            _skipDiagnosticLogs = skipDiagnosticLogs;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        private ILogger CreateLoggerImplementation(string name)
        {
            return new Log4NetLogger(_applicationId, _applicationName, new FileInfo(_log4NetConfigFile), _skipDiagnosticLogs);
        }
    }
}
