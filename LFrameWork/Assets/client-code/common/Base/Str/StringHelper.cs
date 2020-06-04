namespace LFrameWork.Base.Str
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
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
    }
}

