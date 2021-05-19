namespace LFrameWork.Base.CNDate
{
    using System;

    public class ChinaDate
    {
        private static long[] lunarInfo = new long[150]
        {
      19416L,
      19168L,
      42352L,
      21717L,
      53856L,
      55632L,
      91476L,
      22176L,
      39632L,
      21970L,
      19168L,
      42422L,
      42192L,
      53840L,
      119381L,
      46400L,
      54944L,
      44450L,
      38320L,
      84343L,
      18800L,
      42160L,
      46261L,
      27216L,
      27968L,
      109396L,
      11104L,
      38256L,
      21234L,
      18800L,
      25958L,
      54432L,
      59984L,
      28309L,
      23248L,
      11104L,
      100067L,
      37600L,
      116951L,
      51536L,
      54432L,
      120998L,
      46416L,
      22176L,
      107956L,
      9680L,
      37584L,
      53938L,
      43344L,
      46423L,
      27808L,
      46416L,
      86869L,
      19872L,
      42448L,
      83315L,
      21200L,
      43432L,
      59728L,
      27296L,
      44710L,
      43856L,
      19296L,
      43748L,
      42352L,
      21088L,
      62051L,
      55632L,
      23383L,
      22176L,
      38608L,
      19925L,
      19152L,
      42192L,
      54484L,
      53840L,
      54616L,
      46400L,
      46496L,
      103846L,
      38320L,
      18864L,
      43380L,
      42160L,
      45690L,
      27216L,
      27968L,
      44870L,
      43872L,
      38256L,
      19189L,
      18800L,
      25776L,
      29859L,
      59984L,
      27480L,
      21952L,
      43872L,
      38613L,
      37600L,
      51552L,
      55636L,
      54432L,
      55888L,
      30034L,
      22176L,
      43959L,
      9680L,
      37584L,
      51893L,
      43344L,
      46240L,
      47780L,
      44368L,
      21977L,
      19360L,
      42416L,
      86390L,
      21168L,
      43312L,
      31060L,
      27296L,
      44368L,
      23378L,
      19296L,
      42726L,
      42208L,
      53856L,
      60005L,
      54576L,
      23200L,
      30371L,
      38608L,
      19415L,
      19152L,
      42192L,
      118966L,
      53840L,
      54560L,
      56645L,
      46496L,
      22224L,
      21938L,
      18864L,
      42359L,
      42160L,
      43600L,
      111189L,
      27936L,
      44448L
        };
        private static int[] year20 = new int[12]
        {
      1,
      4,
      1,
      2,
      1,
      2,
      1,
      1,
      2,
      1,
      2,
      1
        };
        private static int[] year19 = new int[12]
        {
      0,
      3,
      0,
      1,
      0,
      1,
      0,
      0,
      1,
      0,
      1,
      0
        };
        private static int[] year2000 = new int[12]
        {
      0,
      3,
      1,
      2,
      1,
      2,
      1,
      1,
      2,
      1,
      2,
      1
        };
        private static string[] nStr1 = new string[13]
        {
      "",
      "正",
      "二",
      "三",
      "四",
      "五",
      "六",
      "七",
      "八",
      "九",
      "十",
      "十一",
      "十二"
        };
        private static string[] Gan = new string[10]
        {
      "甲",
      "乙",
      "丙",
      "丁",
      "戊",
      "己",
      "庚",
      "辛",
      "壬",
      "癸"
        };
        private static string[] Zhi = new string[12]
        {
      "子",
      "丑",
      "寅",
      "卯",
      "辰",
      "巳",
      "午",
      "未",
      "申",
      "酉",
      "戌",
      "亥"
        };
        private static string[] Animals = new string[12]
        {
      "鼠",
      "牛",
      "虎",
      "兔",
      "龙",
      "蛇",
      "马",
      "羊",
      "猴",
      "鸡",
      "狗",
      "猪"
        };
        private static string[] solarTerm = new string[24]
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
        private static string[] lFtv = new string[11]
        {
      "0101农历春节",
      "0202 龙抬头节",
      "0115 元宵节",
      "0505 端午节",
      "0707 七夕情人节",
      "0815 中秋节",
      "0909 重阳节",
      "1208 腊八节",
      "1114 李君先生生日",
      "1224 小年",
      "0100除夕"
        };
        private static string[] sFtv = new string[96]
        {
      "0101 新年元旦",
      "0202 世界湿地日",
      "0207 国际声援南非日",
      "0210 国际气象节",
      "0214 情人节",
      "0301 国际海豹日",
      "0303 全国爱耳日",
      "0308 国际妇女节",
      "0312 植树节 孙中山逝世纪念日",
      "0314 国际警察日",
      "0315 国际消费者权益日",
      "0317 中国国医节 国际航海日",
      "0321 世界森林日 消除种族歧视国际日",
      "0321 世界儿歌日",
      "0322 世界水日",
      "0323 世界气象日",
      "0324 世界防治结核病日",
      "0325 全国中小学生安全教育日",
      "0330 巴勒斯坦国土日",
      "0401 愚人节 全国爱国卫生运动月(四月) 税收宣传月(四月)",
      "0407 世界卫生日",
      "0422 世界地球日",
      "0423 世界图书和版权日",
      "0424 亚非新闻工作者日",
      "0501 国际劳动节",
      "0504 中国五四青年节",
      "0505 碘缺乏病防治日",
      "0508 世界红十字日",
      "0512 国际护士节",
      "0515 国际家庭日",
      "0517 世界电信日",
      "0518 国际博物馆日",
      "0520 全国学生营养日",
      "0523 国际牛奶日",
      "0531 世界无烟日",
      "0601 国际儿童节",
      "0605 世界环境日",
      "0606 全国爱眼日",
      "0617 防治荒漠化和干旱日",
      "0623 国际奥林匹克日",
      "0625 全国土地日",
      "0626 国际反毒品日",
      "0701 中国共产党建党日 世界建筑日",
      "0702 国际体育记者日",
      "0707 中国人民抗日战争纪念日",
      "0711 世界人口日",
      "0730 非洲妇女日",
      "0801 中国建军节",
      "0808 中国男子节(爸爸节)",
      "0815 日本正式宣布无条件投降日",
      "0908 国际扫盲日 国际新闻工作者日",
      "0910 教师节",
      "0914 世界清洁地球日",
      "0916 国际臭氧层保护日",
      "0918 九·一八事变纪念日",
      "0920 全国爱牙日",
      "0927 世界旅游日",
      "1001 国庆节 世界音乐日 国际老人节",
      "1001 国际音乐日",
      "1002 国际和平与民主自由斗争日",
      "1004 世界动物日",
      "1008 全国高血压日",
      "1008 世界视觉日",
      "1009 世界邮政日 万国邮联日",
      "1010 辛亥革命纪念日 世界精神卫生日",
      "1013 世界保健日 国际教师节",
      "1014 世界标准日",
      "1015 国际盲人节(白手杖节)",
      "1016 世界粮食日",
      "1017 世界消除贫困日",
      "1022 世界传统医药日",
      "1024 联合国日 世界发展信息日",
      "1031 世界勤俭日",
      "1107 十月社会主义革命纪念日",
      "1108 中国记者日",
      "1109 全国消防安全宣传教育日",
      "1110 世界青年节",
      "1111 国际科学与和平周(本日所属的一周)",
      "1112 孙中山诞辰纪念日",
      "1114 世界糖尿病日",
      "1117 国际大学生节 世界学生节",
      "1121 世界问候日 世界电视日",
      "1129 国际声援巴勒斯坦人民国际日",
      "1201 世界艾滋病日",
      "1203 世界残疾人日",
      "1205 国际经济和社会发展志愿人员日",
      "1208 国际儿童电视日",
      "1209 世界足球日",
      "1210 世界人权日",
      "1212 西安事变纪念日",
      "1213 南京大屠杀(1937年)纪念日！紧记血泪史！",
      "1221 国际篮球日",
      "1224 平安夜",
      "1225 圣诞节",
      "1226 毛主席诞辰",
      "1229 国际生物多样性日"
        };

        private static int lYearDays(int y)
        {
            int num = 348;
            for (int index = 32768; index > 8; index >>= 1)
            {
                if ((ChinaDate.lunarInfo[y - 1900] & (long)index) != 0L)
                    ++num;
            }
            return num + ChinaDate.leapDays(y);
        }

        private static int leapDays(int y)
        {
            if (ChinaDate.leapMonth(y) == 0)
                return 0;
            return (ChinaDate.lunarInfo[y - 1900] & 65536L) != 0L ? 30 : 29;
        }

        private static int leapMonth(int y) => (int)(ChinaDate.lunarInfo[y - 1900] & 15L);

        private static int monthDays(int y, int m) => (ChinaDate.lunarInfo[y - 1900] & (long)(65536 >> m)) == 0L ? 29 : 30;

        private static string AnimalsYear(int y) => ChinaDate.Animals[(y - 4) % 12];

        private static string cyclicalm(int num) => ChinaDate.Gan[num % 10] + ChinaDate.Zhi[num % 12];

        private static string cyclical(int y) => ChinaDate.cyclicalm(y - 1900 + 36);

        private long[] Lunar(int y, int m)
        {
            long[] numArray = new long[7];
            int num1 = 0;
            DateTime dateTime = new DateTime(3800, 2, 31);
            long totalDays = (long)(new DateTime(y + 1900, m + 1, 1) - dateTime).TotalDays;
            if (y < 2000)
                totalDays += (long)ChinaDate.year19[m - 1];
            if (y > 2000)
                totalDays += (long)ChinaDate.year20[m - 1];
            if (y == 2000)
                totalDays += (long)ChinaDate.year2000[m - 1];
            numArray[5] = totalDays + 40L;
            numArray[4] = 14L;
            int y1;
            for (y1 = 1900; y1 < 2050 && totalDays > 0L; ++y1)
            {
                num1 = ChinaDate.lYearDays(y1);
                totalDays -= (long)num1;
                numArray[4] += 12L;
            }
            if (totalDays < 0L)
            {
                totalDays += (long)num1;
                --y1;
                numArray[4] -= 12L;
            }
            numArray[0] = (long)y1;
            numArray[3] = (long)(y1 - 1864);
            int num2 = ChinaDate.leapMonth(y1);
            numArray[6] = 0L;
            int m1;
            for (m1 = 1; m1 < 13 && totalDays > 0L; ++m1)
            {
                if (num2 > 0 && m1 == num2 + 1 && numArray[6] == 0L)
                {
                    --m1;
                    numArray[6] = 1L;
                    num1 = ChinaDate.leapDays((int)numArray[0]);
                }
                else
                    num1 = ChinaDate.monthDays((int)numArray[0], m1);
                if (numArray[6] == 1L && m1 == num2 + 1)
                    numArray[6] = 0L;
                totalDays -= (long)num1;
                if (numArray[6] == 0L)
                    ++numArray[4];
            }
            if (totalDays == 0L && num2 > 0 && m1 == num2 + 1)
            {
                if (numArray[6] == 1L)
                {
                    numArray[6] = 0L;
                }
                else
                {
                    numArray[6] = 1L;
                    --m1;
                    --numArray[4];
                }
            }
            if (totalDays < 0L)
            {
                totalDays += (long)num1;
                --m1;
                --numArray[4];
            }
            numArray[1] = (long)m1;
            numArray[2] = totalDays + 1L;
            return numArray;
        }

        private static long[] calElement(int y, int m, int d)
        {
            long[] numArray = new long[7];
            int num1 = 0;
            DateTime dateTime = new DateTime(1900, 1, 31);
            long totalDays = (long)(new DateTime(y, m, d) - dateTime).TotalDays;
            numArray[5] = totalDays + 40L;
            numArray[4] = 14L;
            int y1;
            for (y1 = 1900; y1 < 2050 && totalDays > 0L; ++y1)
            {
                num1 = ChinaDate.lYearDays(y1);
                totalDays -= (long)num1;
                numArray[4] += 12L;
            }
            if (totalDays < 0L)
            {
                totalDays += (long)num1;
                --y1;
                numArray[4] -= 12L;
            }
            numArray[0] = (long)y1;
            numArray[3] = (long)(y1 - 1864);
            int num2 = ChinaDate.leapMonth(y1);
            numArray[6] = 0L;
            int m1;
            for (m1 = 1; m1 < 13 && totalDays > 0L; ++m1)
            {
                if (num2 > 0 && m1 == num2 + 1 && numArray[6] == 0L)
                {
                    --m1;
                    numArray[6] = 1L;
                    num1 = ChinaDate.leapDays((int)numArray[0]);
                }
                else
                    num1 = ChinaDate.monthDays((int)numArray[0], m1);
                if (numArray[6] == 1L && m1 == num2 + 1)
                    numArray[6] = 0L;
                totalDays -= (long)num1;
                if (numArray[6] == 0L)
                    ++numArray[4];
            }
            if (totalDays == 0L && num2 > 0 && m1 == num2 + 1)
            {
                if (numArray[6] == 1L)
                {
                    numArray[6] = 0L;
                }
                else
                {
                    numArray[6] = 1L;
                    --m1;
                    --numArray[4];
                }
            }
            if (totalDays < 0L)
            {
                totalDays += (long)num1;
                --m1;
                --numArray[4];
            }
            numArray[1] = (long)m1;
            numArray[2] = totalDays + 1L;
            return numArray;
        }

        private static string getChinaDate(int day)
        {
            string str = "";
            switch (day)
            {
                case 10:
                    return "初十";
                case 20:
                    return "二十";
                case 30:
                    return "三十";
                default:
                    int num = day / 10;
                    if (num == 0)
                        str = "初";
                    if (num == 1)
                        str = "十";
                    if (num == 2)
                        str = "廿";
                    if (num == 3)
                        str = "三";
                    switch (day % 10)
                    {
                        case 1:
                            str += "一";
                            break;
                        case 2:
                            str += "二";
                            break;
                        case 3:
                            str += "三";
                            break;
                        case 4:
                            str += "四";
                            break;
                        case 5:
                            str += "五";
                            break;
                        case 6:
                            str += "六";
                            break;
                        case 7:
                            str += "七";
                            break;
                        case 8:
                            str += "八";
                            break;
                        case 9:
                            str += "九";
                            break;
                    }
                    return str;
            }
        }

        private static DateTime sTerm(int y, int n)
        {
            double num1 = 31556925974.7 * (double)(y - 1900);
            double num2 = (double)ChinaDate.sTermInfo[n];
            DateTime dateTime = new DateTime(1900, 1, 6, 2, 5, 0);
            dateTime = dateTime.AddMilliseconds(num1);
            dateTime = dateTime.AddMinutes(num2);
            return dateTime;
        }

        private static string FormatDate(int m, int d) => string.Format("{0:00}{1:00}", (object)m, (object)d);

        public static int GetDaysByMonth(int y, int m) => new int[12]
        {
      31,
      DateTime.IsLeapYear(y) ? 29 : 28,
      31,
      30,
      31,
      30,
      31,
      31,
      30,
      31,
      30,
      31
        }[m - 1];

        public static DateTime GetMondayDateByDate(DateTime dt)
        {
            double num = 0.0;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    num = -6.0;
                    break;
                case DayOfWeek.Tuesday:
                    num = -1.0;
                    break;
                case DayOfWeek.Wednesday:
                    num = -2.0;
                    break;
                case DayOfWeek.Thursday:
                    num = -3.0;
                    break;
                case DayOfWeek.Friday:
                    num = -4.0;
                    break;
                case DayOfWeek.Saturday:
                    num = -5.0;
                    break;
            }
            return dt.AddDays(num);
        }

        public static LFrameWork.Base.CNDate.CNDate getChinaDate(DateTime dt)
        {
            LFrameWork.Base.CNDate.CNDate cnDate = new LFrameWork.Base.CNDate.CNDate();
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            long[] numArray1 = ChinaDate.calElement(year, month, day);
            cnDate.cnIntYear = (int)numArray1[0];
            cnDate.cnIntMonth = (int)numArray1[1];
            cnDate.cnIntDay = (int)numArray1[2];
            cnDate.cnStrYear = ChinaDate.cyclical(year);
            cnDate.cnAnm = ChinaDate.AnimalsYear(year);
            cnDate.cnStrMonth = ChinaDate.nStr1[(int)numArray1[1]] + "月";
            cnDate.cnStrDay = ChinaDate.getChinaDate((int)numArray1[2]);
            string str1 = dt.ToString("MMdd");
            string str2 = ChinaDate.FormatDate(cnDate.cnIntMonth, cnDate.cnIntDay);
            for (int n = 0; n < ChinaDate.solarTerm.Length; ++n)
            {
                if (ChinaDate.sTerm(dt.Year, n).ToString("MMdd").Equals(dt.ToString("MMdd")))
                {
                    cnDate.cnSolarTerm = ChinaDate.solarTerm[n];
                    break;
                }
            }
            foreach (string str3 in ChinaDate.sFtv)
            {
                if (str3.Substring(0, 4).Equals(str1))
                {
                    cnDate.cnFtvs = str3.Substring(4, str3.Length - 4);
                    break;
                }
            }
            foreach (string str3 in ChinaDate.lFtv)
            {
                if (str3.Substring(0, 4).Equals(str2))
                {
                    cnDate.cnFtvl = str3.Substring(4, str3.Length - 4);
                    break;
                }
            }
            dt = dt.AddDays(1.0);
            long[] numArray2 = ChinaDate.calElement(dt.Year, dt.Month, dt.Day);
            if (ChinaDate.FormatDate((int)numArray2[1], (int)numArray2[2]).Equals("0101"))
                cnDate.cnFtvl = "除夕";
            return cnDate;
        }
    }
}

