using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    public class FirstLine
    {
        /// <summary>
        /// The format of the first line as regular expression </summary>
        private const string EXPRESSION = "// <!-- <mdb:mork:z v=\\\"(\\d*)\\.(\\d*)\"/> -->";

        /// <summary>
        /// A real example of the line being parsed </summary>
        private const string SAMPLE = "// <!-- <mdb:mork:z v=\"1.4\"/> -->";

        /// <summary>
        /// Precompiled RegEx Pattern </summary>
        //private static readonly Pattern PATTERN = Pattern.compile(EXPRESSION);

        /// <summary>
        /// The primary version number, e.g. "1" </summary>
        private string majorVersion;

        /// <summary>
        /// THe secondary version number, e.g. "4" </summary>
        private string minorVersion;

        /// <summary>
        /// Concatenated version string, e.g. "1.4" </summary>
        private string version;

        /// <summary>
        /// Parse the Mork entry line which specifies the format and the version
        /// information used by the Mork database file
        /// </summary>
        /// <param name="value">
        ///            the first line of the Mork database file </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are ignored unless the option to convert to C# 7.2 'in' parameters is selected:
        //ORIGINAL LINE: public FirstLine(final String value)
        public FirstLine(string value)
        {
            Match matcher = Regex.Match(EXPRESSION, value);
            if (!matcher.Success)
            {
                throw new Exception("Invalid Mork format: " + value + ", should be: " + SAMPLE);
            }
            majorVersion = matcher.Groups[0].Value;
            minorVersion = matcher.Groups[1].Value;
            version = majorVersion + "." + minorVersion;
        }

        /// <summary>
        /// Returns the Version identifier of the Mork Database file in the format
        /// <code>X.Y</code> where X is the major version identifier and Y is the
        /// minor version identifier
        /// </summary>
        /// <returns> the version of the Mork file, e.g. <code>1.4</code> </returns>
        public virtual string Version
        {
            get
            {
                return version;
            }
        }
    }
}
