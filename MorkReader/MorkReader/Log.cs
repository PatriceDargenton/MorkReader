using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
	/// A simple logging class
	/// 
	/// @author mhaller
	/// </summary>
	internal class Log
    {

        private string sourceClazz;

        public Log(object source)
        {
            if (source == null)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                sourceClazz = typeof(object).FullName;
            }
            else
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                sourceClazz = source.GetType().FullName;
            }
        }

        public virtual string SourceClassname
        {
            get
            {
                return sourceClazz;
            }
        }

        /// <summary>
        /// Logs a warning-level message and returns the full message
        /// </summary>
        /// <param name="message">
        ///            the human-readable message text to print </param>
        /// <param name="throwable">
        ///            an optional exception
        /// @return </param>
        public virtual string warn(string message, Exception throwable)
        {
            StringBuilder writer = new StringBuilder();
            //JAVA TO C# CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
            string output = string.Format("WARN %tT [%s]: %s", DateTime.Now, sourceClazz, message);
            writer.Append(output);
            if (throwable != null)
            {
                writer.Append("\n");
                //throwable.printStackTrace(new PrintWriter(writer));
                Console.Write(writer.ToString());
            }
            string content = writer.ToString();
            Console.Error.WriteLine(content);
            return content;
        }

    }

}
