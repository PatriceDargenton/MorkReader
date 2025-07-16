using System;
using System.IO;

namespace _4n6MorkReader
{
    /// <summary>
    /// Parses literal Mork data and unescapes bytes according to the format
    /// specification.
    /// 
    /// @author mhaller
    /// </summary>
    public class LiteralParser
    {

        /// <summary>
        /// Parses the given content as Mork Literal data
        /// </summary>
        /// <param name="reader">
        ///            the Mork-escaped content </param>
        /// <returns> unescaped literal content value </returns>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public String parse(java.io.Reader reader) throws java.io.IOException
        public virtual string parse(StreamReader reader)
        {
            MemoryStream buf = new MemoryStream(64);
            do
            {
                int c = reader.Read();
                if (c == -1)
                {
                    break;
                }
                // End of literal
                if (c == ')')
                {
                    break;
                }
                // Resolve hex-encoded literal value
                byte[] byteArray = null;
                if (c == '$')
                {
                    char[] cbuf = new char[2];
                    int i = reader.Read(cbuf, 0, cbuf.Length);
                    if (i != 2)
                    {
                        throw new Exception("Could not read Hex-Encoded Literal");
                    }
                    byteArray = new byte[] { (byte)Convert.ToInt32(new string(cbuf), 16) };
                    buf.Write(byteArray, 0, byteArray.Length);
                    continue;
                }
                // Resolve Escaping
                if (c == '\\')
                {
                    int d = reader.Read();
                    if (d == '\n' || d == '\r')
                    {
                        continue;
                    }
                    else
                    {
                        byteArray = new byte[] { (byte)d };
                        buf.Write(byteArray, 0, byteArray.Length);
                        continue;
                    }
                }
                // In all other cases - plain character
                byteArray = new byte[] { (byte)c };
                buf.Write(byteArray, 0, byteArray.Length);
            } while (true);
            return buf.ToString();
        }

    }

}
