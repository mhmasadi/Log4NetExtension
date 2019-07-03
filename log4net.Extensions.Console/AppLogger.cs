using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.Generic;
using log4net.Extension;

namespace log4net.Extensions.Console
{
    public class AppLogger
    {
        ILogger _logger;

        Log4NetLogger _log4NetLogger;

        public AppLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AppLogger>();
        }       

        private string arghandler(params Log4NetProperty[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in args)
                sb.Append(" {" + item.Key + "} ");
            return sb.ToString();
        }

        public void Info(string message, params Log4NetProperty[] args)
        {
            var argdescription = arghandler(args);
            _logger.LogInformation($"{message} : {argdescription}", args);
        }

        public void Debug(string message, params Log4NetProperty[] args)
        {
            var argdescription = arghandler(args);
            _logger.LogDebug($"{message} : {argdescription}", args);
        }

        public void Warning(string message, List<Log4NetProperty> args = null, [CallerFilePath] string path = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            args.Add(new Log4NetProperty { Key = "path", Value = $"{occurePoint(path, line, method)}" });
            var argdescription = arghandler(args.ToArray());
            _logger.LogWarning($"{message} : {argdescription}", args.ToArray());
        }

        public void Error(string message, Exception exception,  List< Log4NetProperty> args=null, [CallerFilePath] string path = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            args.Add(new Log4NetProperty { Key = "path", Value = $"{occurePoint(path, line, method)}" });
            var argdescription = arghandler(args.ToArray());
            _logger.LogError(exception, $"{message} : {argdescription}", args.ToArray());
        }

        private static string occurePoint(string path, int line, string method)
        {
            var file = new FileInfo(path);
            return $"{file.Name}:{line} ({method})";
        }
    }
}
