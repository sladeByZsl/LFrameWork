namespace LFrameWork.Base.TimeAPI
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class TimeHelper
    {
        public static DateTime ConvertTimeStampToTime(this double timeStamp)
        {
            return TimeZoneInfo.ConvertTime(new DateTime(0x7b2, 1, 1), TimeZoneInfo.Local);
        }

        public static double ConvertTimeToTimestamp(this DateTime time)
        {
            TimeSpan span = (TimeSpan) (time - TimeZoneInfo.ConvertTime(new DateTime(0x7b2, 1, 1), TimeZoneInfo.Local));
            return span.TotalSeconds;
        }

        public static bool DateDiff(DateTime DateTime1, DateTime DateTime2, out TimeSpan result)
        {
            if ((DateTime1 == DateTime.MinValue) || (DateTime2 == DateTime.MinValue))
            {
                result = TimeSpan.Zero;
                return false;
            }
            try
            {
                TimeSpan span = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts = new TimeSpan(DateTime2.Ticks);
                result = span.Subtract(ts).Duration();
            }
            catch
            {
                result = TimeSpan.Zero;
                return false;
            }
            return true;
        }

        public static bool FormatDate(DateTime dateTime1, int dateMode, out string result)
        {
            if (((dateTime1 == DateTime.MinValue) || (dateMode < 0)) || (dateMode > 9))
            {
                result = string.Empty;
                return false;
            }
            switch (dateMode)
            {
                case 0:
                    result = dateTime1.ToString("yyyy-MM-dd");
                    break;

                case 1:
                    result = dateTime1.ToString("yyyy-MM-dd HH:mm:ss");
                    break;

                case 2:
                    result = dateTime1.ToString("yyyy/MM/dd");
                    break;

                case 3:
                    result = dateTime1.ToString("yyyy年MM月dd日");
                    break;

                case 4:
                    result = dateTime1.ToString("MM-dd");
                    break;

                case 5:
                    result = dateTime1.ToString("MM/dd");
                    break;

                case 6:
                    result = dateTime1.ToString("MM月dd日");
                    break;

                case 7:
                    result = dateTime1.ToString("yyyy-MM");
                    break;

                case 8:
                    result = dateTime1.ToString("yyyy/MM");
                    break;

                case 9:
                    result = dateTime1.ToString("yyyy年MM月");
                    break;

                default:
                    result = dateTime1.ToString();
                    break;
            }
            return true;
        }

        public static string FormatTime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string FormatTime(this double datetime)
        {
            return DateTime.FromBinary((long) datetime).FormatTime();
        }

        public static bool GetFormatDate(DateTime dt, char Separator, out string result)
        {
            if ((dt != DateTime.MinValue) && !dt.Equals(DBNull.Value))
            {
                string format = string.Format("yyyy{0}MM{1}dd", Separator, Separator);
                result = dt.ToString(format);
                return true;
            }
            return GetFormatDate(DateTime.Now, Separator, out result);
        }

        public static bool GetFormatTime(DateTime dt, char Separator, out string result)
        {
            if ((dt != DateTime.MinValue) && !dt.Equals(DBNull.Value))
            {
                string format = string.Format("hh{0}mm{1}ss", Separator, Separator);
                result = dt.ToString(format);
                return true;
            }
            return GetFormatDate(DateTime.Now, Separator, out result);
        }

        public static bool GetMonthLastDate(int year, int month, out int result)
        {
            if (((year < 1) || (year > 0x270f)) || ((month < 1) || (month > 12)))
            {
                result = 0;
                return false;
            }
            try
            {
                DateTime time = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
                result = time.Day;
            }
            catch
            {
                result = -1;
                return false;
            }
            return true;
        }

        public static bool TimeDiff(DateTime DateTime1, DateTime DateTime2, out string result)
        {
            result = string.Empty;
            try
            {
                TimeSpan span = (TimeSpan) (DateTime2 - DateTime1);
                if (span.Days >= 1)
                {
                    result = span.Days.ToString() + "日";
                }
                else if (span.Hours > 1)
                {
                    result = span.Hours.ToString() + "小时前";
                }
                else
                {
                    result = span.Minutes.ToString() + "分钟前";
                }
            }
            catch
            {
                result = string.Empty;
                return false;
            }
            return true;
        }
    }
}

