namespace LFrameWork.Base.Str
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringHelper
    {
        public static bool CountString(string srcStr, string mark, out int count, int startIndex = 0)
        {
            count = 0;
            if (string.IsNullOrEmpty(srcStr) || string.IsNullOrEmpty(mark))
            {
                return false;
            }
            string str = srcStr;
            while (str.IndexOf(mark, startIndex) >= 0)
            {
                string result = string.Empty;
                if (str.ReplaceFirst(mark, "", out result, startIndex))
                {
                    str = result;
                    count++;
                }
            }
            return true;
        }

        public static bool DelLastChar(string srcStr, string mark, out string result)
        {
            if (string.IsNullOrEmpty(srcStr))
            {
                result = string.Empty;
                return false;
            }
            result = srcStr.Substring(0, srcStr.LastIndexOf(mark));
            return true;
        }

        public static bool QuickValidate(string _express, string _value)
        {
            if (_value == null)
            {
                return false;
            }
            Regex regex = new Regex(_express);
            if (_value.Length == 0)
            {
                return false;
            }
            if (!regex.IsMatch(_value))
            {
                return false;
            }
            return true;
        }

        public static bool ReplaceFirst(this string srcStr, string oldValue, string newValue, out string result, int startIndex = 0)
        {
            if (string.IsNullOrEmpty(srcStr))
            {
                result = string.Empty;
                return false;
            }
            if (string.IsNullOrEmpty(oldValue))
            {
                result = srcStr;
                return false;
            }
            if (oldValue.Length > srcStr.Length)
            {
                result = srcStr;
                return false;
            }
            int index = srcStr.IndexOf(oldValue, startIndex);
            if (index < 0)
            {
                result = srcStr;
                return false;
            }
            result = string.Format("{0}{1}{2}", srcStr.Substring(0, index), newValue, srcStr.Substring(index + oldValue.Length));
            return true;
        }

        public static IEnumerable<byte> ToBytes(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToByteArray(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToUtf8(this string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

        public static byte[] HexToBytes(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            var hexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = "";
                byteValue += hexString[index * 2];
                byteValue += hexString[index * 2 + 1];
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return hexAsBytes;
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ListToString<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in list)
            {
                sb.Append(t);
                sb.Append(",");
            }
            return sb.ToString();
        }
    }
}

