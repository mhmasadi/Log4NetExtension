using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net.Extension;

namespace log4net.Extensions.Console
{
   public  class Class1
    {
        AppLogger _appLogger;

        public Class1(AppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public int MyProperty { get; set; }

        public string name{ get; set; }

        public void Run()
        {
            try
            {
                int nd = 0;
                int m = 10;

                int y =m/ nd ;
                var n = Thread.CurrentThread.ManagedThreadId;
                var objects = new Log4NetProperty[] {
                    new Log4NetProperty { Key = "liid", Value = "546546" },
                    new Log4NetProperty { Key = "liid", Value = "546546" } };
                _appLogger.Info("Attempted to divide by zero.", objects);

            }
            catch (Exception ex)
            {
                var objects = new Log4NetProperty[] {
                    new Log4NetProperty { Key = "liid", Value = "546546" },
                    new Log4NetProperty { Key = "liid", Value = "546546" } };
                _appLogger.Error(ex.Message,ex, objects.ToList());
            }

          
        }
    }
}
