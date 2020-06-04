namespace LFrameWork.Base.Str
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public static class NumberFormatter
    {
        private static readonly string[] unitArray = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };

        public static string BriefNumber(long num, int[] divitionPosition, string[] divitionSign, int integerCount, int decimalPointCount)
        {
            int exponent = divitionPosition[0];
            int index = 0;
            double num4 = num;
            if (((divitionPosition.Length == divitionSign.Length) && (integerCount >= 1)) && (decimalPointCount >= 0))
            {
                while (num4 >= Pow(10.0, exponent))
                {
                    double num5 = num4;
                    num4 /= Pow(10.0, exponent);
                    if (Convert.ToInt64(num4).ToString().Length < integerCount)
                    {
                        index--;
                        num4 = num5;
                        break;
                    }
                    if (index >= (divitionPosition.Length - 1))
                    {
                        index++;
                        break;
                    }
                    index++;
                    exponent = divitionPosition[index] - divitionPosition[index - 1];
                }
            }
            else
            {
                return string.Empty;
            }
            string input = num4.ToString("F" + decimalPointCount);
            if (input.Contains("."))
            {
                input = Regex.Replace(input, "(0+)$", "");
                if (input.EndsWith("."))
                {
                    input = input.Substring(0, input.Length - 1);
                }
            }
            if (index > 0)
            {
                return (input + divitionSign[index - 1]);
            }
            return input;
        }

        private static string BytesConvertCommon(double num)
        {
            string str;
            int index = 0;
            double num3 = num;
            while (num3 >= 1000.0)
            {
                if (index >= (unitArray.Length - 1))
                {
                    break;
                }
                num3 /= 1024.0;
                index++;
            }
            if (num3 > 1000.0)
            {
                str = ((int) num3).ToString();
            }
            else
            {
                str = num3.ToString("G3");
            }
            return (str + unitArray[index]);
        }

        private static double Pow(double baseNum, int exponent)
        {
            if ((baseNum < 0.0) || (exponent < 0))
            {
                return 0.0;
            }
            double num = 1.0;
            int num2 = 0;
            int num3 = exponent;
            while (num2 < num3)
            {
                num *= baseNum;
                num2++;
            }
            return num;
        }

        public static string ToBytesString(this int num)
        {
            return BytesConvertCommon((double) num);
        }

        public static string ToBytesString(this long num)
        {
            return BytesConvertCommon((double) num);
        }

        public static string ToBytesString(this uint num)
        {
            return BytesConvertCommon((double) num);
        }

        public static string ToBytesString(this ulong num)
        {
            return BytesConvertCommon((double) num);
        }
    }
}

