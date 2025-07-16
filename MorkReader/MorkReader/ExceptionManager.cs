using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    public class ExceptionManager 
    {
        private static ExceptionHandler exceptionHandler = new DefaultExceptionHandler();

        public static String createString(String value, Exception t)
        {
            exceptionHandler.handle(t);
            return value;
        }

        public static void setExceptionHandler(ExceptionHandler exceptionHandler)
        {
            ExceptionManager.exceptionHandler = exceptionHandler;
        }
    }
}
