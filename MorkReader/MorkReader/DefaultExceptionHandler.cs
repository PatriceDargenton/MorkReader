using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    internal class DefaultExceptionHandler : ExceptionHandler
    {
        public virtual void handle(Exception t)
        {
            if (t is Exception)
                throw (Exception)t;
            else
                throw new Exception(t.Message);
        }

    }
}
