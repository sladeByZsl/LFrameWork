namespace LFrameWork.Base.CNDate
{
    using System;
    using System.Runtime.InteropServices;

    public class ChineseCalendar
    {
        private int _cDay;
        private static string[] _chineseConstellationName = new string[] { 
            "角木蛟", "亢金龙", "女土蝠", "房日兔", "心月狐", "尾火虎", "箕水豹", "斗木獬", "牛金牛", "氐土貉", "虚日鼠", "危月燕", "室火猪", "壁水獝", "奎木狼", "娄金狗", 
            "胃土彘", "昴日鸡", "毕月乌", "觜火猴", "参水猿", "井木犴", "鬼金羊", "柳土獐", "星日马", "张月鹿", "翼火蛇", "轸水蚓"
         };
        private bool _cIsLeapMonth;
        private bool _cIsLeapYear;
        private int _cMonth;
        private static string[] _constellationName = new string[] { "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "摩羯座", "水瓶座", "双鱼座" };
        private int _cYear;
        private DateTime _date;
        private DateTime _datetime;
        private static string[] _monthString = new string[] { "出错", "正月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "腊月" };
        private const int AnimalStartYear = 0x76c;
        private static string animalStr = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private static DateTime ChineseConstellationReferDay = new DateTime(0x7d7, 9, 13);
        private static string ganStr = "甲乙丙丁戊己庚辛壬癸";
        private static DateTime GanZhiStartDay = new DateTime(0x76b, 12, 0x16);
        private const int GanZhiStartYear = 0x748;
        private const string HZNum = "零一二三四五六七八九";
        private static LunarHolidayStruct[] lHolidayInfo = new LunarHolidayStruct[] { new LunarHolidayStruct(1, 1, 1, "春节"), new LunarHolidayStruct(1, 15, 0, "元宵节"), new LunarHolidayStruct(5, 5, 0, "端午节"), new LunarHolidayStruct(7, 7, 0, "七夕情人节"), new LunarHolidayStruct(7, 15, 0, "中元节 盂兰盆节"), new LunarHolidayStruct(8, 15, 0, "中秋节"), new LunarHolidayStruct(9, 9, 0, "重阳节"), new LunarHolidayStruct(12, 8, 0, "腊八节"), new LunarHolidayStruct(12, 0x17, 0, "北方小年(扫房)"), new LunarHolidayStruct(12, 0x18, 0, "南方小年(掸尘)") };
        private static int[] LunarDateArray = new int[] { 
            0x4bd8, 0x4ae0, 0xa570, 0x54d5, 0xd260, 0xd950, 0x16554, 0x56a0, 0x9ad0, 0x55d2, 0x4ae0, 0xa5b6, 0xa4d0, 0xd250, 0x1d255, 0xb540, 
            0xd6a0, 0xada2, 0x95b0, 0x14977, 0x4970, 0xa4b0, 0xb4b5, 0x6a50, 0x6d40, 0x1ab54, 0x2b60, 0x9570, 0x52f2, 0x4970, 0x6566, 0xd4a0, 
            0xea50, 0x6e95, 0x5ad0, 0x2b60, 0x186e3, 0x92e0, 0x1c8d7, 0xc950, 0xd4a0, 0x1d8a6, 0xb550, 0x56a0, 0x1a5b4, 0x25d0, 0x92d0, 0xd2b2, 
            0xa950, 0xb557, 0x6ca0, 0xb550, 0x15355, 0x4da0, 0xa5b0, 0x14573, 0x52b0, 0xa9a8, 0xe950, 0x6aa0, 0xaea6, 0xab50, 0x4b60, 0xaae4, 
            0xa570, 0x5260, 0xf263, 0xd950, 0x5b57, 0x56a0, 0x96d0, 0x4dd5, 0x4ad0, 0xa4d0, 0xd4d4, 0xd250, 0xd558, 0xb540, 0xb6a0, 0x195a6, 
            0x95b0, 0x49b0, 0xa974, 0xa4b0, 0xb27a, 0x6a50, 0x6d40, 0xaf46, 0xab60, 0x9570, 0x4af5, 0x4970, 0x64b0, 0x74a3, 0xea50, 0x6b58, 
            0x55c0, 0xab60, 0x96d5, 0x92e0, 0xc960, 0xd954, 0xd4a0, 0xda50, 0x7552, 0x56a0, 0xabb7, 0x25d0, 0x92d0, 0xcab5, 0xa950, 0xb4a0, 
            0xbaa4, 0xad50, 0x55d9, 0x4ba0, 0xa5b0, 0x15176, 0x52b0, 0xa930, 0x7954, 0x6aa0, 0xad50, 0x5b52, 0x4b60, 0xa6e6, 0xa4e0, 0xd260, 
            0xea65, 0xd530, 0x5aa0, 0x76a3, 0x96d0, 0x4bd7, 0x4ad0, 0xa4d0, 0x1d0b6, 0xd250, 0xd520, 0xdd45, 0xb5a0, 0x56d0, 0x55b2, 0x49b0, 
            0xa577, 0xa4b0, 0xaa50, 0x1b255, 0x6d20, 0xada0, 0x14b63
         };
        private static DateTime MaxDay = new DateTime(0x801, 12, 0x1f);
        private const int MaxYear = 0x802;
        private static DateTime MinDay = new DateTime(0x76c, 1, 30);
        private const int MinYear = 0x76c;
        private static string nStr1 = "日一二三四五六七八九";
        private static string nStr2 = "初十廿卅";
        private static SolarHolidayStruct[] sHolidayInfo = new SolarHolidayStruct[] { 
            new SolarHolidayStruct(1, 1, 1, "元旦"), new SolarHolidayStruct(2, 2, 0, "世界湿地日"), new SolarHolidayStruct(2, 10, 0, "国际气象节"), new SolarHolidayStruct(2, 14, 0, "情人节"), new SolarHolidayStruct(3, 1, 0, "国际海豹日"), new SolarHolidayStruct(3, 5, 0, "学雷锋纪念日"), new SolarHolidayStruct(3, 8, 0, "妇女节"), new SolarHolidayStruct(3, 12, 0, "植树节 孙中山逝世纪念日"), new SolarHolidayStruct(3, 14, 0, "国际警察日"), new SolarHolidayStruct(3, 15, 0, "消费者权益日"), new SolarHolidayStruct(3, 0x11, 0, "中国国医节 国际航海日"), new SolarHolidayStruct(3, 0x15, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"), new SolarHolidayStruct(3, 0x16, 0, "世界水日"), new SolarHolidayStruct(3, 0x18, 0, "世界防治结核病日"), new SolarHolidayStruct(4, 1, 0, "愚人节"), new SolarHolidayStruct(4, 7, 0, "世界卫生日"), 
            new SolarHolidayStruct(4, 0x16, 0, "世界地球日"), new SolarHolidayStruct(5, 1, 1, "劳动节"), new SolarHolidayStruct(5, 2, 1, "劳动节假日"), new SolarHolidayStruct(5, 3, 1, "劳动节假日"), new SolarHolidayStruct(5, 4, 0, "青年节"), new SolarHolidayStruct(5, 8, 0, "世界红十字日"), new SolarHolidayStruct(5, 12, 0, "国际护士节"), new SolarHolidayStruct(5, 0x1f, 0, "世界无烟日"), new SolarHolidayStruct(6, 1, 0, "国际儿童节"), new SolarHolidayStruct(6, 5, 0, "世界环境保护日"), new SolarHolidayStruct(6, 0x1a, 0, "国际禁毒日"), new SolarHolidayStruct(7, 1, 0, "建党节 香港回归纪念 世界建筑日"), new SolarHolidayStruct(7, 11, 0, "世界人口日"), new SolarHolidayStruct(8, 1, 0, "建军节"), new SolarHolidayStruct(8, 8, 0, "中国男子节 父亲节"), new SolarHolidayStruct(8, 15, 0, "抗日战争胜利纪念"), 
            new SolarHolidayStruct(9, 9, 0, "毛主席逝世纪念"), new SolarHolidayStruct(9, 10, 0, "教师节"), new SolarHolidayStruct(9, 0x12, 0, "九\x00b7一八事变纪念日"), new SolarHolidayStruct(9, 20, 0, "国际爱牙日"), new SolarHolidayStruct(9, 0x1b, 0, "世界旅游日"), new SolarHolidayStruct(9, 0x1c, 0, "孔子诞辰"), new SolarHolidayStruct(10, 1, 1, "国庆节 国际音乐日"), new SolarHolidayStruct(10, 2, 1, "国庆节假日"), new SolarHolidayStruct(10, 3, 1, "国庆节假日"), new SolarHolidayStruct(10, 6, 0, "老人节"), new SolarHolidayStruct(10, 0x18, 0, "联合国日"), new SolarHolidayStruct(11, 10, 0, "世界青年节"), new SolarHolidayStruct(11, 12, 0, "孙中山诞辰纪念"), new SolarHolidayStruct(12, 1, 0, "世界艾滋病日"), new SolarHolidayStruct(12, 3, 0, "世界残疾人日"), new SolarHolidayStruct(12, 20, 0, "澳门回归纪念"), 
            new SolarHolidayStruct(12, 0x18, 0, "平安夜"), new SolarHolidayStruct(12, 0x19, 0, "圣诞节"), new SolarHolidayStruct(12, 0x1a, 0, "毛主席诞辰纪念")
         };
        private static string[] SolarTerm = new string[] { 
            "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", 
            "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
         };
        private static int[] sTermInfo = new int[] { 
            0, 0x52d8, 0xa5e3, 0xf95c, 0x14d59, 0x1a206, 0x1f763, 0x24d89, 0x2a45d, 0x2fbdf, 0x353d8, 0x3ac35, 0x404af, 0x45d25, 0x4b553, 0x50d19, 
            0x56446, 0x5bac6, 0x61087, 0x6658a, 0x6b9db, 0x70d90, 0x760cc, 0x7b3b6
         };
        private static WeekHolidayStruct[] wHolidayInfo = new WeekHolidayStruct[] { new WeekHolidayStruct(5, 2, 1, "母亲节"), new WeekHolidayStruct(5, 3, 1, "全国助残日"), new WeekHolidayStruct(6, 3, 1, "父亲节"), new WeekHolidayStruct(9, 3, 3, "国际和平日"), new WeekHolidayStruct(9, 4, 1, "国际聋人节"), new WeekHolidayStruct(10, 1, 2, "国际住房日"), new WeekHolidayStruct(10, 1, 4, "国际减轻自然灾害日"), new WeekHolidayStruct(11, 4, 5, "感恩节") };
        private static string zhiStr = "子丑寅卯辰巳午未申酉戌亥";

        public ChineseCalendar(DateTime dt)
        {
            this.CheckDateLimit(dt);
            this._date = dt.Date;
            this._datetime = dt;
            int chineseLeapMonth = 0;
            int chineseYearDays = 0;
            TimeSpan span = (TimeSpan) (this._date - MinDay);
            int days = span.Days;
            int year = 0x76c;
            while (year <= 0x802)
            {
                chineseYearDays = this.GetChineseYearDays(year);
                if ((days - chineseYearDays) < 1)
                {
                    break;
                }
                days -= chineseYearDays;
                year++;
            }
            this._cYear = year;
            chineseLeapMonth = this.GetChineseLeapMonth(this._cYear);
            if (chineseLeapMonth > 0)
            {
                this._cIsLeapYear = true;
            }
            else
            {
                this._cIsLeapYear = false;
            }
            this._cIsLeapMonth = false;
            year = 1;
            while (year <= 12)
            {
                if (((chineseLeapMonth > 0) && (year == (chineseLeapMonth + 1))) && !this._cIsLeapMonth)
                {
                    this._cIsLeapMonth = true;
                    year--;
                    chineseYearDays = this.GetChineseLeapMonthDays(this._cYear);
                }
                else
                {
                    this._cIsLeapMonth = false;
                    chineseYearDays = this.GetChineseMonthDays(this._cYear, year);
                }
                days -= chineseYearDays;
                if (days <= 0)
                {
                    break;
                }
                year++;
            }
            days += chineseYearDays;
            this._cMonth = year;
            this._cDay = days;
        }

        public ChineseCalendar(int cy, int cm, int cd, bool leapMonthFlag)
        {
            int num;
            int chineseYearDays;
            this.CheckChineseDateLimit(cy, cm, cd, leapMonthFlag);
            this._cYear = cy;
            this._cMonth = cm;
            this._cDay = cd;
            int num4 = 0;
            for (num = 0x76c; num < cy; num++)
            {
                chineseYearDays = this.GetChineseYearDays(num);
                num4 += chineseYearDays;
            }
            int chineseLeapMonth = this.GetChineseLeapMonth(cy);
            if (chineseLeapMonth != 0)
            {
                this._cIsLeapYear = true;
            }
            else
            {
                this._cIsLeapYear = false;
            }
            if (cm != chineseLeapMonth)
            {
                this._cIsLeapMonth = false;
            }
            else
            {
                this._cIsLeapMonth = leapMonthFlag;
            }
            if (!this._cIsLeapYear || (cm < chineseLeapMonth))
            {
                for (num = 1; num < cm; num++)
                {
                    chineseYearDays = this.GetChineseMonthDays(cy, num);
                    num4 += chineseYearDays;
                }
                if (cd > this.GetChineseMonthDays(cy, cm))
                {
                    throw new Exception("不合法的农历日期");
                }
                num4 += cd;
            }
            else
            {
                for (num = 1; num < cm; num++)
                {
                    chineseYearDays = this.GetChineseMonthDays(cy, num);
                    num4 += chineseYearDays;
                }
                if (cm > chineseLeapMonth)
                {
                    chineseYearDays = this.GetChineseLeapMonthDays(cy);
                    num4 += chineseYearDays;
                    if (cd > this.GetChineseMonthDays(cy, cm))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    num4 += cd;
                }
                else
                {
                    if (this._cIsLeapMonth)
                    {
                        chineseYearDays = this.GetChineseMonthDays(cy, cm);
                        num4 += chineseYearDays;
                    }
                    if (cd > this.GetChineseLeapMonthDays(cy))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    num4 += cd;
                }
            }
            this._date = MinDay.AddDays((double) num4);
        }

        private bool BitTest32(int num, int bitpostion)
        {
            if ((bitpostion > 0x1f) || (bitpostion < 0))
            {
                throw new Exception("Error Param: bitpostion[0-31]:" + bitpostion.ToString());
            }
            int num2 = ((int) 1) << bitpostion;
            if ((num & num2) == 0)
            {
                return false;
            }
            return true;
        }

        private void CheckChineseDateLimit(int year, int month, int day, bool leapMonth)
        {
            if ((year < 0x76c) || (year > 0x802))
            {
                throw new Exception("非法农历日期");
            }
            if ((month < 1) || (month > 12))
            {
                throw new Exception("非法农历日期");
            }
            if ((day < 1) || (day > 30))
            {
                throw new Exception("非法农历日期");
            }
            int chineseLeapMonth = this.GetChineseLeapMonth(year);
            if (leapMonth && (month != chineseLeapMonth))
            {
                throw new Exception("非法农历日期");
            }
        }

        private void CheckDateLimit(DateTime dt)
        {
            if ((dt < MinDay) || (dt > MaxDay))
            {
                throw new Exception("超出可转换的日期");
            }
        }

        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            bool flag = false;
            if ((date.Month == month) && (this.ConvertDayOfWeek(date.DayOfWeek) == day))
            {
                DateTime time = new DateTime(date.Year, date.Month, 1);
                int num = (7 - this.ConvertDayOfWeek(time.DayOfWeek)) + 1;
                if (this.ConvertDayOfWeek(time.DayOfWeek) > day)
                {
                    if (((((week - 1) * 7) + day) + num) == date.Day)
                    {
                        flag = true;
                    }
                    return flag;
                }
                if (((day + num) + ((week - 2) * 7)) == date.Day)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;

                case DayOfWeek.Monday:
                    return 2;

                case DayOfWeek.Tuesday:
                    return 3;

                case DayOfWeek.Wednesday:
                    return 4;

                case DayOfWeek.Thursday:
                    return 5;

                case DayOfWeek.Friday:
                    return 6;

                case DayOfWeek.Saturday:
                    return 7;
            }
            return 0;
        }

        private string ConvertNumToChineseNum(char n)
        {
            if ((n >= '0') && (n <= '9'))
            {
                char ch;
                switch (n)
                {
                    case '0':
                        ch = "零一二三四五六七八九"[0];
                        return ch.ToString();

                    case '1':
                        ch = "零一二三四五六七八九"[1];
                        return ch.ToString();

                    case '2':
                        ch = "零一二三四五六七八九"[2];
                        return ch.ToString();

                    case '3':
                        ch = "零一二三四五六七八九"[3];
                        return ch.ToString();

                    case '4':
                        ch = "零一二三四五六七八九"[4];
                        return ch.ToString();

                    case '5':
                        ch = "零一二三四五六七八九"[5];
                        return ch.ToString();

                    case '6':
                        ch = "零一二三四五六七八九"[6];
                        return ch.ToString();

                    case '7':
                        ch = "零一二三四五六七八九"[7];
                        return ch.ToString();

                    case '8':
                        ch = "零一二三四五六七八九"[8];
                        return ch.ToString();

                    case '9':
                        ch = "零一二三四五六七八九"[9];
                        return ch.ToString();
                }
            }
            return "";
        }

        private string GetChineseHour(DateTime dt)
        {
            int hour = dt.Hour;
            if (dt.Minute != 0)
            {
                hour++;
            }
            int num2 = hour / 2;
            if (num2 >= 12)
            {
                num2 = 0;
            }
            TimeSpan span = (TimeSpan) (this._date - GanZhiStartDay);
            int startIndex = ((((((span.Days % 60) % 10) + 1) * 2) - 1) % 10) - 1;
            char ch = (ganStr.Substring(startIndex) + ganStr.Substring(0, startIndex + 2))[num2];
            ch = zhiStr[num2];
            string introduced5 = ch.ToString();
            return (introduced5 + ch.ToString());
        }

        private int GetChineseLeapMonth(int year)
        {
            return (LunarDateArray[year - 0x76c] & 15);
        }

        private int GetChineseLeapMonthDays(int year)
        {
            if (this.GetChineseLeapMonth(year) == 0)
            {
                return 0;
            }
            if ((LunarDateArray[year - 0x76c] & 0x10000) != 0)
            {
                return 30;
            }
            return 0x1d;
        }

        private int GetChineseMonthDays(int year, int month)
        {
            if (this.BitTest32(LunarDateArray[year - 0x76c] & 0xffff, 0x10 - month))
            {
                return 30;
            }
            return 0x1d;
        }

        private int GetChineseYearDays(int year)
        {
            int num2 = 0x15c;
            int num = 0x8000;
            int num3 = LunarDateArray[year - 0x76c] & 0xffff;
            for (int i = 0; i < 12; i++)
            {
                if ((num3 & num) != 0)
                {
                    num2++;
                }
                num = num >> 1;
            }
            return (num2 + this.GetChineseLeapMonthDays(year));
        }

        public int Animal
        {
            get
            {
                return (((this._date.Year - 0x76c) % 12) + 1);
            }
        }

        public string AnimalString
        {
            get
            {
                int num = this._date.Year - 0x76c;
                char ch = animalStr[num % 12];
                return ch.ToString();
            }
        }

        public string ChineseConstellation
        {
            get
            {
                int index = 0;
                TimeSpan span = (TimeSpan) (this._date - ChineseConstellationReferDay);
                index = span.Days % 0x1c;
                if (index < 0)
                {
                    return _chineseConstellationName[0x1b + index];
                }
                return _chineseConstellationName[index];
            }
        }

        public string ChineseDateString
        {
            get
            {
                if (this._cIsLeapMonth)
                {
                    string[] textArray1 = new string[] { "农历", this.ChineseYearString, "闰", this.ChineseMonthString, this.ChineseDayString };
                    return string.Concat(textArray1);
                }
                return ("农历" + this.ChineseYearString + this.ChineseMonthString + this.ChineseDayString);
            }
        }

        public int ChineseDay
        {
            get
            {
                return this._cDay;
            }
        }

        public string ChineseDayString
        {
            get
            {
                switch (this._cDay)
                {
                    case 0:
                        return "";

                    case 10:
                        return "初十";

                    case 20:
                        return "二十";

                    case 30:
                        return "三十";
                }
                char ch = nStr2[this._cDay / 10];
                ch = nStr1[this._cDay % 10];
                string introduced2 = ch.ToString();
                return (introduced2 + ch.ToString());
            }
        }

        public string ChineseHour
        {
            get
            {
                return this.GetChineseHour(this._datetime);
            }
        }

        public int ChineseMonth
        {
            get
            {
                return this._cMonth;
            }
        }

        public string ChineseMonthString
        {
            get
            {
                return _monthString[this._cMonth];
            }
        }

        public string ChineseTwentyFourDay
        {
            get
            {
                DateTime time = new DateTime(0x76c, 1, 6, 2, 5, 0);
                int year = this._date.Year;
                for (int i = 1; i <= 0x18; i++)
                {
                    double num = (525948.76 * (year - 0x76c)) + sTermInfo[i - 1];
                    if (time.AddMinutes(num).DayOfYear == this._date.DayOfYear)
                    {
                        return SolarTerm[i - 1];
                    }
                }
                return "";
            }
        }

        public string ChineseTwentyFourNextDay
        {
            get
            {
                DateTime time = new DateTime(0x76c, 1, 6, 2, 5, 0);
                int year = this._date.Year;
                for (int i = 1; i <= 0x18; i++)
                {
                    double num = (525948.76 * (year - 0x76c)) + sTermInfo[i - 1];
                    DateTime time2 = time.AddMinutes(num);
                    if (time2.DayOfYear > this._date.DayOfYear)
                    {
                        return string.Format("{0}[{1}]", SolarTerm[i - 1], time2.ToString("yyyy-MM-dd"));
                    }
                }
                return "";
            }
        }

        public string ChineseTwentyFourPrevDay
        {
            get
            {
                DateTime time = new DateTime(0x76c, 1, 6, 2, 5, 0);
                int year = this._date.Year;
                for (int i = 0x18; i >= 1; i--)
                {
                    double num = (525948.76 * (year - 0x76c)) + sTermInfo[i - 1];
                    DateTime time2 = time.AddMinutes(num);
                    if (time2.DayOfYear < this._date.DayOfYear)
                    {
                        return string.Format("{0}[{1}]", SolarTerm[i - 1], time2.ToString("yyyy-MM-dd"));
                    }
                }
                return "";
            }
        }

        public int ChineseYear
        {
            get
            {
                return this._cYear;
            }
        }

        public string ChineseYearString
        {
            get
            {
                string str = "";
                string str2 = this._cYear.ToString();
                for (int i = 0; i < 4; i++)
                {
                    str = str + this.ConvertNumToChineseNum(str2[i]);
                }
                return (str + "年");
            }
        }

        public string Constellation
        {
            get
            {
                int index = 0;
                int year = this._date.Year;
                int day = this._date.Day;
                year = (this._date.Month * 100) + day;
                if ((year >= 0x141) && (year <= 0x1a3))
                {
                    index = 0;
                }
                else if ((year >= 420) && (year <= 520))
                {
                    index = 1;
                }
                else if ((year >= 0x209) && (year <= 620))
                {
                    index = 2;
                }
                else if ((year >= 0x26d) && (year <= 0x2d2))
                {
                    index = 3;
                }
                else if ((year >= 0x2d3) && (year <= 0x336))
                {
                    index = 4;
                }
                else if ((year >= 0x337) && (year <= 0x39a))
                {
                    index = 5;
                }
                else if ((year >= 0x39b) && (year <= 0x3fe))
                {
                    index = 6;
                }
                else if ((year >= 0x3ff) && (year <= 0x461))
                {
                    index = 7;
                }
                else if ((year >= 0x462) && (year <= 0x4c5))
                {
                    index = 8;
                }
                else if ((year >= 0x4c6) || (year <= 0x77))
                {
                    index = 9;
                }
                else if ((year >= 120) && (year <= 0xda))
                {
                    index = 10;
                }
                else if ((year >= 0xdb) && (year <= 320))
                {
                    index = 11;
                }
                else
                {
                    index = 0;
                }
                return _constellationName[index];
            }
        }

        public DateTime Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
            }
        }

        public string DateHoliday
        {
            get
            {
                foreach (SolarHolidayStruct struct2 in sHolidayInfo)
                {
                    if ((struct2.Month == this._date.Month) && (struct2.Day == this._date.Day))
                    {
                        return struct2.HolidayName;
                    }
                }
                return "";
            }
        }

        public string DateString
        {
            get
            {
                return ("公元" + this._date.ToLongDateString());
            }
        }

        public string GanZhiDateString
        {
            get
            {
                return (this.GanZhiYearString + this.GanZhiMonthString + this.GanZhiDayString);
            }
        }

        public string GanZhiDayString
        {
            get
            {
                TimeSpan span = (TimeSpan) (this._date - GanZhiStartDay);
                int num = span.Days % 60;
                char ch = ganStr[num % 10];
                ch = zhiStr[num % 12];
                string introduced3 = ch.ToString();
                return (introduced3 + ch.ToString() + "日");
            }
        }

        public string GanZhiMonthString
        {
            get
            {
                int num;
                if (this._cMonth > 10)
                {
                    num = this._cMonth - 10;
                }
                else
                {
                    num = this._cMonth + 2;
                }
                string str = zhiStr[num - 1].ToString();
                int num2 = 1;
                switch ((((this._cYear - 0x748) % 60) % 10))
                {
                    case 0:
                        num2 = 3;
                        break;

                    case 1:
                        num2 = 5;
                        break;

                    case 2:
                        num2 = 7;
                        break;

                    case 3:
                        num2 = 9;
                        break;

                    case 4:
                        num2 = 1;
                        break;

                    case 5:
                        num2 = 3;
                        break;

                    case 6:
                        num2 = 5;
                        break;

                    case 7:
                        num2 = 7;
                        break;

                    case 8:
                        num2 = 9;
                        break;

                    case 9:
                        num2 = 1;
                        break;
                }
                char ch = ganStr[((num2 + this._cMonth) - 2) % 10];
                return (ch.ToString() + str + "月");
            }
        }

        public string GanZhiYearString
        {
            get
            {
                int num = (this._cYear - 0x748) % 60;
                char ch = ganStr[num % 10];
                ch = zhiStr[num % 12];
                string introduced2 = ch.ToString();
                return (introduced2 + ch.ToString() + "年");
            }
        }

        public bool IsChineseLeapMonth
        {
            get
            {
                return this._cIsLeapMonth;
            }
        }

        public bool IsChineseLeapYear
        {
            get
            {
                return this._cIsLeapYear;
            }
        }

        public bool IsLeapYear
        {
            get
            {
                return DateTime.IsLeapYear(this._date.Year);
            }
        }

        public string newCalendarHoliday
        {
            get
            {
                string holidayName = "";
                if (!this._cIsLeapMonth)
                {
                    foreach (LunarHolidayStruct struct2 in lHolidayInfo)
                    {
                        if ((struct2.Month == this._cMonth) && (struct2.Day == this._cDay))
                        {
                            holidayName = struct2.HolidayName;
                            break;
                        }
                    }
                    if (this._cMonth == 12)
                    {
                        int chineseMonthDays = this.GetChineseMonthDays(this._cYear, 12);
                        if (this._cDay == chineseMonthDays)
                        {
                            holidayName = "除夕";
                        }
                    }
                }
                return holidayName;
            }
        }

        public DayOfWeek WeekDay
        {
            get
            {
                return this._date.DayOfWeek;
            }
        }

        public string WeekDayHoliday
        {
            get
            {
                foreach (WeekHolidayStruct struct2 in wHolidayInfo)
                {
                    if (this.CompareWeekDayHoliday(this._date, struct2.Month, struct2.WeekAtMonth, struct2.WeekDay))
                    {
                        return struct2.HolidayName;
                    }
                }
                return "";
            }
        }

        public string WeekDayStr
        {
            get
            {
                switch (this._date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "星期日";

                    case DayOfWeek.Monday:
                        return "星期一";

                    case DayOfWeek.Tuesday:
                        return "星期二";

                    case DayOfWeek.Wednesday:
                        return "星期三";

                    case DayOfWeek.Thursday:
                        return "星期四";

                    case DayOfWeek.Friday:
                        return "星期五";
                }
                return "星期六";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LunarHolidayStruct
        {
            public int Month;
            public int Day;
            public int Recess;
            public string HolidayName;
            public LunarHolidayStruct(int month, int day, int recess, string name)
            {
                this.Month = month;
                this.Day = day;
                this.Recess = recess;
                this.HolidayName = name;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SolarHolidayStruct
        {
            public int Month;
            public int Day;
            public int Recess;
            public string HolidayName;
            public SolarHolidayStruct(int month, int day, int recess, string name)
            {
                this.Month = month;
                this.Day = day;
                this.Recess = recess;
                this.HolidayName = name;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WeekHolidayStruct
        {
            public int Month;
            public int WeekAtMonth;
            public int WeekDay;
            public string HolidayName;
            public WeekHolidayStruct(int month, int weekAtMonth, int weekDay, string name)
            {
                this.Month = month;
                this.WeekAtMonth = weekAtMonth;
                this.WeekDay = weekDay;
                this.HolidayName = name;
            }
        }
    }
}

