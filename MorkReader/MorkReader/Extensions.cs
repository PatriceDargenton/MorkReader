using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorkReader
{
    public static class Extensions
    {
        public static string ToUTF8String(this sbyte[] bytes)
        {
            return ToUTF8String(bytes, 0, bytes.Length);
        }
        public static string ToUTF8String(this sbyte[] bytes, int index, int count)
        {
            return System.Text.Encoding.UTF8.GetString((byte[])(object)bytes, index, count);
        }

        public static MemoryStream ToMemoryStream(this string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""));
        }
    }
}
