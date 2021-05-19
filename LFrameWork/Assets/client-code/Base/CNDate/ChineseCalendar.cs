namespace LFrameWork.Base.CNDate
{
    using System;
    using System.Runtime.InteropServices;

    public class ChineseCalendar
    {
        private DateTime _date;
        private DateTime _datetime;
        private int _cYear;
        private int _cMonth;
        private int _cDay;
        private bool _cIsLeapMonth;
        private bool _cIsLeapYear;
        private const int MinYear = 1900;
        private const int MaxYear = 2050;
        private static DateTime MinDay = new DateTime(1900, 1, 30);
        private static DateTime MaxDay = new DateTime(2049, 12, 31);
        private const int GanZhiStartYear = 1864;
        private static DateTime GanZhiStartDay = new DateTime(1899, 12, 22);
        private const string HZNum = "零一二三四五六七八九";
        private const int AnimalStartYear = 1900;
        private static DateTime ChineseConstellationReferDay = new DateTime(2007, 9, 13);
        private static int[] LunarDateArray = new int[151]
        {
      19416,
      19168,
      42352,
      21717,
      53856,
      55632,
      91476,
      22176,
      39632,
      21970,
      19168,
      42422,
      42192,
      53840,
      119381,
      46400,
      54944,
      44450,
      38320,
      84343,
      18800,
      42160,
      46261,
      27216,
      27968,
      109396,
      11104,
      38256,
      21234,
      18800,
      25958,
      54432,
      59984,
      28309,
      23248,
      11104,
      100067,
      37600,
      116951,
      51536,
      54432,
      120998,
      46416,
      22176,
      107956,
      9680,
      37584,
      53938,
      43344,
      46423,
      27808,
      46416,
      86869,
      19872,
      42416,
      83315,
      21168,
      43432,
      59728,
      27296,
      44710,
      43856,
      19296,
      43748,
      42352,
      21088,
      62051,
      55632,
      23383,
      22176,
      38608,
      19925,
      19152,
      42192,
      54484,
      53840,
      54616,
      46400,
      46752,
      103846,
      38320,
      18864,
      43380,
      42160,
      45690,
      27216,
      27968,
      44870,
      43872,
      38256,
      19189,
      18800,
      25776,
      29859,
      59984,
      27480,
      21952,
      43872,
      38613,
      37600,
      51552,
      55636,
      54432,
      55888,
      30034,
      22176,
      43959,
      9680,
      37584,
      51893,
      43344,
      46240,
      47780,
      44368,
      21977,
      19360,
      42416,
      86390,
      21168,
      43312,
      31060,
      27296,
      44368,
      23378,
      19296,
      42726,
      42208,
      53856,
      60005,
      54576,
      23200,
      30371,
      38608,
      19415,
      19152,
      42192,
      118966,
      53840,
      54560,
      56645,
      46496,
      22224,
      21938,
      18864,
      42359,
      42160,
      43600,
      111189,
      27936,
      44448,
      84835
        };
        private static string[] _constellationName = new string[12]
        {
      "白羊座",
      "金牛座",
      "双子座",
      "巨蟹座",
      "狮子座",
      "处女座",
      "天秤座",
      "天蝎座",
      "射手座",
      "摩羯座",
      "水瓶座",
      "双鱼座"
        };
        private static string[] _chineseConstellationName = new string[28]
        {
      "角木蛟",
      "亢金龙",
      "女土蝠",
      "房日兔",
      "心月狐",
      "尾火虎",
      "箕水豹",
      "斗木獬",
      "牛金牛",
      "氐土貉",
      "虚日鼠",
      "危月燕",
      "室火猪",
      "壁水獝",
      "奎木狼",
      "娄金狗",
      "胃土彘",
      "昴日鸡",
      "毕月乌",
      "觜火猴",
      "参水猿",
      "井木犴",
      "鬼金羊",
      "柳土獐",
      "星日马",
      "张月鹿",
      "翼火蛇",
      "轸水蚓"
        };
        private static string[] SolarTerm = new string[24]
        {
      "小寒",
      "大寒",
      "立春",
      "雨水",
      "惊蛰",
      "春分",
      "清明",
      "谷雨",
      "立夏",
      "小满",
      "芒种",
      "夏至",
      "小暑",
      "大暑",
      "立秋",
      "处暑",
      "白露",
      "秋分",
      "寒露",
      "霜降",
      "立冬",
      "小雪",
      "大雪",
      "冬至"
        };
        private static int[] sTermInfo = new int[24]
        {
      0,
      21208,
      42467,
      63836,
      85337,
      107014,
      128867,
      150921,
      173149,
      195551,
      218072,
      240693,
      263343,
      285989,
      308563,
      331033,
      353350,
      375494,
      397447,
      419210,
      440795,
      462224,
      483532,
      504758
        };
        private static string ganStr = "甲乙丙丁戊己庚辛壬癸";
        private static string zhiStr = "子丑寅卯辰巳午未申酉戌亥";
        private static string animalStr = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private static string nStr1 = "日一二三四五六七八九";
        private static string nStr2 = "初十廿卅";
        private static string[] _monthString = new string[13]
        {
      "出错",
      "正月",
      "二月",
      "三月",
      "四月",
      "五月",
      "六月",
      "七月",
      "八月",
      "九月",
      "十月",
      "十一月",
      "腊月"
        };
        private static ChineseCalendar.SolarHolidayStruct[] sHolidayInfo = new ChineseCalendar.SolarHolidayStruct[51]
        {
      new ChineseCalendar.SolarHolidayStruct(1, 1, 1, "元旦"),
      new ChineseCalendar.SolarHolidayStruct(2, 2, 0, "世界湿地日"),
      new ChineseCalendar.SolarHolidayStruct(2, 10, 0, "国际气象节"),
      new ChineseCalendar.SolarHolidayStruct(2, 14, 0, "情人节"),
      new ChineseCalendar.SolarHolidayStruct(3, 1, 0, "国际海豹日"),
      new ChineseCalendar.SolarHolidayStruct(3, 5, 0, "学雷锋纪念日"),
      new ChineseCalendar.SolarHolidayStruct(3, 8, 0, "妇女节"),
      new ChineseCalendar.SolarHolidayStruct(3, 12, 0, "植树节 孙中山逝世纪念日"),
      new ChineseCalendar.SolarHolidayStruct(3, 14, 0, "国际警察日"),
      new ChineseCalendar.SolarHolidayStruct(3, 15, 0, "消费者权益日"),
      new ChineseCalendar.SolarHolidayStruct(3, 17, 0, "中国国医节 国际航海日"),
      new ChineseCalendar.SolarHolidayStruct(3, 21, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"),
      new ChineseCalendar.SolarHolidayStruct(3, 22, 0, "世界水日"),
      new ChineseCalendar.SolarHolidayStruct(3, 24, 0, "世界防治结核病日"),
      new ChineseCalendar.SolarHolidayStruct(4, 1, 0, "愚人节"),
      new ChineseCalendar.SolarHolidayStruct(4, 7, 0, "世界卫生日"),
      new ChineseCalendar.SolarHolidayStruct(4, 22, 0, "世界地球日"),
      new ChineseCalendar.SolarHolidayStruct(5, 1, 1, "劳动节"),
      new ChineseCalendar.SolarHolidayStruct(5, 2, 1, "劳动节假日"),
      new ChineseCalendar.SolarHolidayStruct(5, 3, 1, "劳动节假日"),
      new ChineseCalendar.SolarHolidayStruct(5, 4, 0, "青年节"),
      new ChineseCalendar.SolarHolidayStruct(5, 8, 0, "世界红十字日"),
      new ChineseCalendar.SolarHolidayStruct(5, 12, 0, "国际护士节"),
      new ChineseCalendar.SolarHolidayStruct(5, 31, 0, "世界无烟日"),
      new ChineseCalendar.SolarHolidayStruct(6, 1, 0, "国际儿童节"),
      new ChineseCalendar.SolarHolidayStruct(6, 5, 0, "世界环境保护日"),
      new ChineseCalendar.SolarHolidayStruct(6, 26, 0, "国际禁毒日"),
      new ChineseCalendar.SolarHolidayStruct(7, 1, 0, "建党节 香港回归纪念 世界建筑日"),
      new ChineseCalendar.SolarHolidayStruct(7, 11, 0, "世界人口日"),
      new ChineseCalendar.SolarHolidayStruct(8, 1, 0, "建军节"),
      new ChineseCalendar.SolarHolidayStruct(8, 8, 0, "中国男子节 父亲节"),
      new ChineseCalendar.SolarHolidayStruct(8, 15, 0, "抗日战争胜利纪念"),
      new ChineseCalendar.SolarHolidayStruct(9, 9, 0, "毛主席逝世纪念"),
      new ChineseCalendar.SolarHolidayStruct(9, 10, 0, "教师节"),
      new ChineseCalendar.SolarHolidayStruct(9, 18, 0, "九·一八事变纪念日"),
      new ChineseCalendar.SolarHolidayStruct(9, 20, 0, "国际爱牙日"),
      new ChineseCalendar.SolarHolidayStruct(9, 27, 0, "世界旅游日"),
      new ChineseCalendar.SolarHolidayStruct(9, 28, 0, "孔子诞辰"),
      new ChineseCalendar.SolarHolidayStruct(10, 1, 1, "国庆节 国际音乐日"),
      new ChineseCalendar.SolarHolidayStruct(10, 2, 1, "国庆节假日"),
      new ChineseCalendar.SolarHolidayStruct(10, 3, 1, "国庆节假日"),
      new ChineseCalendar.SolarHolidayStruct(10, 6, 0, "老人节"),
      new ChineseCalendar.SolarHolidayStruct(10, 24, 0, "联合国日"),
      new ChineseCalendar.SolarHolidayStruct(11, 10, 0, "世界青年节"),
      new ChineseCalendar.SolarHolidayStruct(11, 12, 0, "孙中山诞辰纪念"),
      new ChineseCalendar.SolarHolidayStruct(12, 1, 0, "世界艾滋病日"),
      new ChineseCalendar.SolarHolidayStruct(12, 3, 0, "世界残疾人日"),
      new ChineseCalendar.SolarHolidayStruct(12, 20, 0, "澳门回归纪念"),
      new ChineseCalendar.SolarHolidayStruct(12, 24, 0, "平安夜"),
      new ChineseCalendar.SolarHolidayStruct(12, 25, 0, "圣诞节"),
      new ChineseCalendar.SolarHolidayStruct(12, 26, 0, "毛主席诞辰纪念")
        };
        private static ChineseCalendar.LunarHolidayStruct[] lHolidayInfo = new ChineseCalendar.LunarHolidayStruct[10]
        {
      new ChineseCalendar.LunarHolidayStruct(1, 1, 1, "春节"),
      new ChineseCalendar.LunarHolidayStruct(1, 15, 0, "元宵节"),
      new ChineseCalendar.LunarHolidayStruct(5, 5, 0, "端午节"),
      new ChineseCalendar.LunarHolidayStruct(7, 7, 0, "七夕情人节"),
      new ChineseCalendar.LunarHolidayStruct(7, 15, 0, "中元节 盂兰盆节"),
      new ChineseCalendar.LunarHolidayStruct(8, 15, 0, "中秋节"),
      new ChineseCalendar.LunarHolidayStruct(9, 9, 0, "重阳节"),
      new ChineseCalendar.LunarHolidayStruct(12, 8, 0, "腊八节"),
      new ChineseCalendar.LunarHolidayStruct(12, 23, 0, "北方小年(扫房)"),
      new ChineseCalendar.LunarHolidayStruct(12, 24, 0, "南方小年(掸尘)")
        };
        private static ChineseCalendar.WeekHolidayStruct[] wHolidayInfo = new ChineseCalendar.WeekHolidayStruct[8]
        {
      new ChineseCalendar.WeekHolidayStruct(5, 2, 1, "母亲节"),
      new ChineseCalendar.WeekHolidayStruct(5, 3, 1, "全国助残日"),
      new ChineseCalendar.WeekHolidayStruct(6, 3, 1, "父亲节"),
      new ChineseCalendar.WeekHolidayStruct(9, 3, 3, "国际和平日"),
      new ChineseCalendar.WeekHolidayStruct(9, 4, 1, "国际聋人节"),
      new ChineseCalendar.WeekHolidayStruct(10, 1, 2, "国际住房日"),
      new ChineseCalendar.WeekHolidayStruct(10, 1, 4, "国际减轻自然灾害日"),
      new ChineseCalendar.WeekHolidayStruct(11, 4, 5, "感恩节")
        };

        public ChineseCalendar(DateTime dt)
        {
            this.CheckDateLimit(dt);
            this._date = dt.Date;
            this._datetime = dt;
            int num1 = 0;
            int days = (this._date - ChineseCalendar.MinDay).Days;
            int year;
            for (year = 1900; year <= 2050; ++year)
            {
                num1 = this.GetChineseYearDays(year);
                if (days - num1 >= 1)
                    days -= num1;
                else
                    break;
            }
            this._cYear = year;
            int chineseLeapMonth = this.GetChineseLeapMonth(this._cYear);
            this._cIsLeapYear = chineseLeapMonth > 0;
            this._cIsLeapMonth = false;
            int month;
            for (month = 1; month <= 12; ++month)
            {
                if (chineseLeapMonth > 0 && month == chineseLeapMonth + 1 && !this._cIsLeapMonth)
                {
                    this._cIsLeapMonth = true;
                    --month;
                    num1 = this.GetChineseLeapMonthDays(this._cYear);
                }
                else
                {
                    this._cIsLeapMonth = false;
                    num1 = this.GetChineseMonthDays(this._cYear, month);
                }
                days -= num1;
                if (days <= 0)
                    break;
            }
            int num2 = days + num1;
            this._cMonth = month;
            this._cDay = num2;
        }

        public ChineseCalendar(int cy, int cm, int cd, bool leapMonthFlag)
        {
            this.CheckChineseDateLimit(cy, cm, cd, leapMonthFlag);
            this._cYear = cy;
            this._cMonth = cm;
            this._cDay = cd;
            int num1 = 0;
            for (int year = 1900; year < cy; ++year)
            {
                int chineseYearDays = this.GetChineseYearDays(year);
                num1 += chineseYearDays;
            }
            int chineseLeapMonth = this.GetChineseLeapMonth(cy);
            this._cIsLeapYear = chineseLeapMonth != 0;
            this._cIsLeapMonth = cm == chineseLeapMonth && leapMonthFlag;
            int num2;
            if (!this._cIsLeapYear || cm < chineseLeapMonth)
            {
                for (int month = 1; month < cm; ++month)
                {
                    int chineseMonthDays = this.GetChineseMonthDays(cy, month);
                    num1 += chineseMonthDays;
                }
                if (cd > this.GetChineseMonthDays(cy, cm))
                    throw new Exception("不合法的农历日期");
                num2 = num1 + cd;
            }
            else
            {
                for (int month = 1; month < cm; ++month)
                {
                    int chineseMonthDays = this.GetChineseMonthDays(cy, month);
                    num1 += chineseMonthDays;
                }
                if (cm > chineseLeapMonth)
                {
                    int chineseLeapMonthDays = this.GetChineseLeapMonthDays(cy);
                    int num3 = num1 + chineseLeapMonthDays;
                    if (cd > this.GetChineseMonthDays(cy, cm))
                        throw new Exception("不合法的农历日期");
                    num2 = num3 + cd;
                }
                else
                {
                    if (this._cIsLeapMonth)
                    {
                        int chineseMonthDays = this.GetChineseMonthDays(cy, cm);
                        num1 += chineseMonthDays;
                    }
                    if (cd > this.GetChineseLeapMonthDays(cy))
                        throw new Exception("不合法的农历日期");
                    num2 = num1 + cd;
                }
            }
            this._date = ChineseCalendar.MinDay.AddDays((double)num2);
        }

        private int GetChineseMonthDays(int year, int month) => this.BitTest32(ChineseCalendar.LunarDateArray[year - 1900] & (int)ushort.MaxValue, 16 - month) ? 30 : 29;

        private int GetChineseLeapMonth(int year) => ChineseCalendar.LunarDateArray[year - 1900] & 15;

        private int GetChineseLeapMonthDays(int year)
        {
            if (this.GetChineseLeapMonth(year) == 0)
                return 0;
            return (ChineseCalendar.LunarDateArray[year - 1900] & 65536) != 0 ? 30 : 29;
        }

        private int GetChineseYearDays(int year)
        {
            int num1 = 348;
            int num2 = 32768;
            int num3 = ChineseCalendar.LunarDateArray[year - 1900] & (int)ushort.MaxValue;
            for (int index = 0; index < 12; ++index)
            {
                if ((num3 & num2) != 0)
                    ++num1;
                num2 >>= 1;
            }
            return num1 + this.GetChineseLeapMonthDays(year);
        }

        private string GetChineseHour(DateTime dt)
        {
            int hour = dt.Hour;
            if (dt.Minute != 0)
                ++hour;
            int index = hour / 2;
            if (index >= 12)
                index = 0;
            int startIndex = (((this._date - ChineseCalendar.GanZhiStartDay).Days % 60 % 10 + 1) * 2 - 1) % 10 - 1;
            char ch = (ChineseCalendar.ganStr.Substring(startIndex) + ChineseCalendar.ganStr.Substring(0, startIndex + 2))[index];
            string str1 = ch.ToString();
            ch = ChineseCalendar.zhiStr[index];
            string str2 = ch.ToString();
            return str1 + str2;
        }

        private void CheckDateLimit(DateTime dt)
        {
            if (dt < ChineseCalendar.MinDay || dt > ChineseCalendar.MaxDay)
                throw new Exception("超出可转换的日期");
        }

        private void CheckChineseDateLimit(int year, int month, int day, bool leapMonth)
        {
            if (year < 1900 || year > 2050)
                throw new Exception("非法农历日期");
            if (month < 1 || month > 12)
                throw new Exception("非法农历日期");
            if (day < 1 || day > 30)
                throw new Exception("非法农历日期");
            int chineseLeapMonth = this.GetChineseLeapMonth(year);
            if (leapMonth && month != chineseLeapMonth)
                throw new Exception("非法农历日期");
        }

        private string ConvertNumToChineseNum(char n)
        {
            if (n < '0' || n > '9')
                return "";
            switch (n)
            {
                case '0':
                    return "零一二三四五六七八九"[0].ToString();
                case '1':
                    return "零一二三四五六七八九"[1].ToString();
                case '2':
                    return "零一二三四五六七八九"[2].ToString();
                case '3':
                    return "零一二三四五六七八九"[3].ToString();
                case '4':
                    return "零一二三四五六七八九"[4].ToString();
                case '5':
                    return "零一二三四五六七八九"[5].ToString();
                case '6':
                    return "零一二三四五六七八九"[6].ToString();
                case '7':
                    return "零一二三四五六七八九"[7].ToString();
                case '8':
                    return "零一二三四五六七八九"[8].ToString();
                case '9':
                    return "零一二三四五六七八九"[9].ToString();
                default:
                    return "";
            }
        }

        private bool BitTest32(int num, int bitpostion)
        {
            if (bitpostion > 31 || bitpostion < 0)
                throw new Exception("Error Param: bitpostion[0-31]:" + bitpostion.ToString());
            int num1 = 1 << bitpostion;
            return (num & num1) != 0;
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
                default:
                    return 0;
            }
        }

        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            bool flag = false;
            if (date.Month == month && this.ConvertDayOfWeek(date.DayOfWeek) == day)
            {
                DateTime dateTime = new DateTime(date.Year, date.Month, 1);
                int num1 = this.ConvertDayOfWeek(dateTime.DayOfWeek);
                int num2 = 7 - this.ConvertDayOfWeek(dateTime.DayOfWeek) + 1;
                int num3 = day;
                if (num1 > num3)
                {
                    if ((week - 1) * 7 + day + num2 == date.Day)
                        flag = true;
                }
                else if (day + num2 + (week - 2) * 7 == date.Day)
                    flag = true;
            }
            return flag;
        }

        public string newCalendarHoliday
        {
            get
            {
                string str = "";
                if (!this._cIsLeapMonth)
                {
                    foreach (ChineseCalendar.LunarHolidayStruct lunarHolidayStruct in ChineseCalendar.lHolidayInfo)
                    {
                        if (lunarHolidayStruct.Month == this._cMonth && lunarHolidayStruct.Day == this._cDay)
                        {
                            str = lunarHolidayStruct.HolidayName;
                            break;
                        }
                    }
                    if (this._cMonth == 12 && this._cDay == this.GetChineseMonthDays(this._cYear, 12))
                        str = "除夕";
                }
                return str;
            }
        }

        public string WeekDayHoliday
        {
            get
            {
                string str = "";
                foreach (ChineseCalendar.WeekHolidayStruct weekHolidayStruct in ChineseCalendar.wHolidayInfo)
                {
                    if (this.CompareWeekDayHoliday(this._date, weekHolidayStruct.Month, weekHolidayStruct.WeekAtMonth, weekHolidayStruct.WeekDay))
                    {
                        str = weekHolidayStruct.HolidayName;
                        break;
                    }
                }
                return str;
            }
        }

        public string DateHoliday
        {
            get
            {
                string str = "";
                foreach (ChineseCalendar.SolarHolidayStruct solarHolidayStruct in ChineseCalendar.sHolidayInfo)
                {
                    if (solarHolidayStruct.Month == this._date.Month && solarHolidayStruct.Day == this._date.Day)
                    {
                        str = solarHolidayStruct.HolidayName;
                        break;
                    }
                }
                return str;
            }
        }

        public DateTime Date
        {
            get => this._date;
            set => this._date = value;
        }

        public DayOfWeek WeekDay => this._date.DayOfWeek;

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
                    default:
                        return "星期六";
                }
            }
        }

        public string DateString => "公元" + this._date.ToLongDateString();

        public bool IsLeapYear => DateTime.IsLeapYear(this._date.Year);

        public string ChineseConstellation
        {
            get
            {
                int index = (this._date - ChineseCalendar.ChineseConstellationReferDay).Days % 28;
                return index < 0 ? ChineseCalendar._chineseConstellationName[27 + index] : ChineseCalendar._chineseConstellationName[index];
            }
        }

        public string ChineseHour => this.GetChineseHour(this._datetime);

        public bool IsChineseLeapMonth => this._cIsLeapMonth;

        public bool IsChineseLeapYear => this._cIsLeapYear;

        public int ChineseDay => this._cDay;

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
                    default:
                        char ch = ChineseCalendar.nStr2[this._cDay / 10];
                        string str1 = ch.ToString();
                        ch = ChineseCalendar.nStr1[this._cDay % 10];
                        string str2 = ch.ToString();
                        return str1 + str2;
                }
            }
        }

        public int ChineseMonth => this._cMonth;

        public string ChineseMonthString => ChineseCalendar._monthString[this._cMonth];

        public int ChineseYear => this._cYear;

        public string ChineseYearString
        {
            get
            {
                string str1 = "";
                string str2 = this._cYear.ToString();
                for (int index = 0; index < 4; ++index)
                    str1 += this.ConvertNumToChineseNum(str2[index]);
                return str1 + "年";
            }
        }

        public string ChineseDateString
        {
            get
            {
                if (!this._cIsLeapMonth)
                    return "农历" + this.ChineseYearString + this.ChineseMonthString + this.ChineseDayString;
                return "农历" + this.ChineseYearString + "闰" + this.ChineseMonthString + this.ChineseDayString;
            }
        }

        public string ChineseTwentyFourDay
        {
            get
            {
                DateTime dateTime = new DateTime(1900, 1, 6, 2, 5, 0);
                string str = "";
                int year = this._date.Year;
                for (int index = 1; index <= 24; ++index)
                {
                    double num = 525948.76 * (double)(year - 1900) + (double)ChineseCalendar.sTermInfo[index - 1];
                    if (dateTime.AddMinutes(num).DayOfYear == this._date.DayOfYear)
                    {
                        str = ChineseCalendar.SolarTerm[index - 1];
                        break;
                    }
                }
                return str;
            }
        }

        public string ChineseTwentyFourPrevDay
        {
            get
            {
                DateTime dateTime1 = new DateTime(1900, 1, 6, 2, 5, 0);
                string str = "";
                int year = this._date.Year;
                for (int index = 24; index >= 1; --index)
                {
                    double num = 525948.76 * (double)(year - 1900) + (double)ChineseCalendar.sTermInfo[index - 1];
                    DateTime dateTime2 = dateTime1.AddMinutes(num);
                    if (dateTime2.DayOfYear < this._date.DayOfYear)
                    {
                        str = string.Format("{0}[{1}]", (object)ChineseCalendar.SolarTerm[index - 1], (object)dateTime2.ToString("yyyy-MM-dd"));
                        break;
                    }
                }
                return str;
            }
        }

        public string ChineseTwentyFourNextDay
        {
            get
            {
                DateTime dateTime1 = new DateTime(1900, 1, 6, 2, 5, 0);
                string str = "";
                int year = this._date.Year;
                for (int index = 1; index <= 24; ++index)
                {
                    double num = 525948.76 * (double)(year - 1900) + (double)ChineseCalendar.sTermInfo[index - 1];
                    DateTime dateTime2 = dateTime1.AddMinutes(num);
                    if (dateTime2.DayOfYear > this._date.DayOfYear)
                    {
                        str = string.Format("{0}[{1}]", (object)ChineseCalendar.SolarTerm[index - 1], (object)dateTime2.ToString("yyyy-MM-dd"));
                        break;
                    }
                }
                return str;
            }
        }

        public string Constellation
        {
            get
            {
                int year = this._date.Year;
                int num = this._date.Month * 100 + this._date.Day;
                int index = num < 321 || num > 419 ? (num < 420 || num > 520 ? (num < 521 || num > 620 ? (num < 621 || num > 722 ? (num < 723 || num > 822 ? (num < 823 || num > 922 ? (num < 923 || num > 1022 ? (num < 1023 || num > 1121 ? (num < 1122 || num > 1221 ? (num >= 1222 || num <= 119 ? 9 : (num < 120 || num > 218 ? (num < 219 || num > 320 ? 0 : 11) : 10)) : 8) : 7) : 6) : 5) : 4) : 3) : 2) : 1) : 0;
                return ChineseCalendar._constellationName[index];
            }
        }

        public int Animal => (this._date.Year - 1900) % 12 + 1;

        public string AnimalString
        {
            get
            {
                int num = this._date.Year - 1900;
                return ChineseCalendar.animalStr[num % 12].ToString();
            }
        }

        public string GanZhiYearString
        {
            get
            {
                int num = (this._cYear - 1864) % 60;
                char ch = ChineseCalendar.ganStr[num % 10];
                string str1 = ch.ToString();
                ch = ChineseCalendar.zhiStr[num % 12];
                string str2 = ch.ToString();
                return str1 + str2 + "年";
            }
        }

        public string GanZhiMonthString
        {
            get
            {
                int num1 = this._cMonth <= 10 ? this._cMonth + 2 : this._cMonth - 10;
                string str = ChineseCalendar.zhiStr[num1 - 1].ToString();
                int num2 = 1;
                switch ((this._cYear - 1864) % 60 % 10)
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
                return ChineseCalendar.ganStr[(num2 + this._cMonth - 2) % 10].ToString() + str + "月";
            }
        }

        public string GanZhiDayString
        {
            get
            {
                int num = (this._date - ChineseCalendar.GanZhiStartDay).Days % 60;
                char ch = ChineseCalendar.ganStr[num % 10];
                string str1 = ch.ToString();
                ch = ChineseCalendar.zhiStr[num % 12];
                string str2 = ch.ToString();
                return str1 + str2 + "日";
            }
        }

        public string GanZhiDateString => this.GanZhiYearString + this.GanZhiMonthString + this.GanZhiDayString;

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

