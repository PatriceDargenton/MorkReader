using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    public class StringUtils
    {

        /// <summary>
        /// Removes C/C++-style single line comments from the given content
        /// </summary>
        /// <param name="value">
        /// @return </param>
        public static string removeCommentLines(string value)
        {
            //Pattern pattern = Pattern.compile("(//.*)([\\r\\n]|$)", Pattern.MULTILINE);
            Match matcher = Regex.Match(value, "(//.*)([\\r\\n]|$)", RegexOptions.Multiline);
            if (matcher.Success)
            {
                return matcher.Result("\n");
            }
            return value;
        }

        /// <summary>
        /// Removes all newlines (Carriage Feed and Newline character) from the value
        /// </summary>
        /// <param name="value">
        /// @return </param>
        public static string removeNewlines(string value)
        {
            return Regex.Replace(value, "[\\n\\r]", "");
        }

        public static string removeDoubleNewlines(string value)
        {
            value = Regex.Replace(value, "[\\n\\r]{2}", "\n");
            value = Regex.Replace(value, "[\\n\\r]{2}", "\n");
            return value;
        }

        /// <summary>
        /// Loads a Classpath Resource and returns it as single String
        /// </summary>
        /// <param name="resourceName">
        ///            the classpath resource to load, relative to the StringUtils
        ///            class. E.g. use "/foo.txt" to load files from the root of the
        ///            classpath. </param>
        /// <returns> a String with the whole content of the file. Newlines are
        ///         replaced with the newline character. </returns>
        /// <exception cref="RuntimeException">
        ///             if there was any IO error with the resource </exception>
        public static string fromResource(string resourceName)
        {
            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            ////ORIGINAL LINE: final BufferedReader reader = new BufferedReader(new InputStreamReader(StringUtils.class.getResourceAsStream(resourceName)));
            //StreamReader reader = new StreamReader(typeof(StringUtils).getResourceAsStream(resourceName));
            //try
            //{
            //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //    //ORIGINAL LINE: final StringBuffer buf = new StringBuffer();
            //    StringBuilder buf = new StringBuilder();
            //    string line = reader.ReadLine();
            //    while (!string.ReferenceEquals(line, null))
            //    {
            //        buf.Append(line);
            //        buf.Append("\n");
            //        line = reader.ReadLine();
            //    }
            //    reader.Close();
            //    return buf.ToString();
            //}
            //catch (IOException e)
            //{
            //    try
            //    {
            //        reader.Close();
            //    }
            //    catch (IOException e1)
            //    {
            //        throw e1;
            //    }
            //    throw e;
            //}
            throw new NotImplementedException("fromResource");
        }

    }

}
