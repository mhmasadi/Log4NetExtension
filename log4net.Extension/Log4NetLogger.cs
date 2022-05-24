using log4net;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace log4net.Extension
{
    public class Log4NetLogger : ILogger
    {
        private readonly int _applicationId;

        private readonly string _applicationName;       

        private readonly ILog _log;

//hello
        private readonly bool _skipDiagnosticLogs;

        private ILoggerRepository _loggerRepository;

        public Log4NetLogger(int applicationId, string applicationName, FileInfo fileInfo, bool skipDiagnosticLogs)
        {
            _applicationName = applicationName;
            _applicationId = applicationId;
            _loggerRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            _log = LogManager.GetLogger(_loggerRepository.Name, applicationName);
            _skipDiagnosticLogs = skipDiagnosticLogs;

            log4net.Config.XmlConfigurator.Configure(_loggerRepository, fileInfo);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _log.IsDebugEnabled && AllowDiagnostics();
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled && AllowDiagnostics();
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception,  string> formatter)
        {
            string message = "";
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var formattedLogValues = state as FormattedLogValues;
            formattedLogValues.AsParallel().ForAll(item => {
                var log4NetProperty = item.Value as Log4NetProperty;
                if (log4NetProperty != null)
                    ThreadContext.Properties[log4NetProperty.Key] = log4NetProperty.Value;
                if (item.Key == "{OriginalFormat}")
                    message = item.Value.ToString().Split(new char[] { ':' })[0];
            });
            //foreach (var item in formattedLogValues)
            //{
            //    var log4NetProperty = item.Value as Log4NetProperty;
            //    if (log4NetProperty != null)
            //        ThreadContext.Properties[log4NetProperty.Key] = log4NetProperty.Value;
            //    if (item.Key == "{OriginalFormat}")
            //        message = item.Value.ToString().Split(new char[] { ':' })[0];
            //}
            
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            log4net.ThreadContext.Properties["applicationId"] = _applicationId;            
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _log.Fatal(message);
                        break;
                    case LogLevel.Debug:
                    case LogLevel.Trace:
                        _log.Debug(message);
                        break;
                    case LogLevel.Error:
                        _log.Error(message);
                        break;
                    case LogLevel.Information:
                        _log.Info(message);
                        break;
                    case LogLevel.Warning:
                        _log.Warn(message);
                        break;
                    default:
                        _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        _log.Info(message, exception);
                        break;
                }
            }
        }

        private bool AllowDiagnostics()
        {
            if (!_skipDiagnosticLogs)
            {
                return true;
            }

            return !(_applicationName.ToLower().StartsWith("microsoft")
                || _applicationName == "IdentityServer4.AccessTokenValidation.Infrastructure.NopAuthenticationMiddleware");
        }
    }
}
