namespace LFrameWork.Base.CNDate
{
    using System;

    public class ChinaDate
    {
        private static string[] Animals = new string[] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        private static string[] Gan = new string[] { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static string[] lFtv = new string[] { "0101农历春节", "0202 龙抬头节", "0115 元宵节", "0505 端午节", "0707 七夕情人节", "0815 中秋节", "0909 重阳节", "1208 腊八节", "1114 李君先生生日", "1224 小年", "0100除夕" };
        private static long[] lunarInfo = new long[] { 
            0x4bd8L, 0x4ae0L, 0xa570L, 0x54d5L, 0xd260L, 0xd950L, 0x16554L, 0x56a0L, 0x9ad0L, 0x55d2L, 0x4ae0L, 0xa5b6L, 0xa4d0L, 0xd250L, 0x1d255L, 0xb540L, 
            0xd6a0L, 0xada2L, 0x95b0L, 0x14977L, 0x4970L, 0xa4b0L, 0xb4b5L, 0x6a50L, 0x6d40L, 0x1ab54L, 0x2b60L, 0x9570L, 0x52f2L, 0x4970L, 0x6566L, 0xd4a0L, 
            0xea50L, 0x6e95L, 0x5ad0L, 0x2b60L, 0x186e3L, 0x92e0L, 0x1c8d7L, 0xc950L, 0xd4a0L, 0x1d8a6L, 0xb550L, 0x56a0L, 0x1a5b4L, 0x25d0L, 0x92d0L, 0xd2b2L, 
            0xa950L, 0xb557L, 0x6ca0L, 0xb550L, 0x15355L, 0x4da0L, 0xa5d0L, 0x14573L, 0x52d0L, 0xa9a8L, 0xe950L, 0x6aa0L, 0xaea6L, 0xab50L, 0x4b60L, 0xaae4L, 
            0xa570L, 0x5260L, 0xf263L, 0xd950L, 0x5b57L, 0x56a0L, 0x96d0L, 0x4dd5L, 0x4ad0L, 0xa4d0L, 0xd4d4L, 0xd250L, 0xd558L, 0xb540L, 0xb5a0L, 0x195a6L, 
            0x95b0L, 0x49b0L, 0xa974L, 0xa4b0L, 0xb27aL, 0x6a50L, 0x6d40L, 0xaf46L, 0xab60L, 0x9570L, 0x4af5L, 0x4970L, 0x64b0L, 0x74a3L, 0xea50L, 0x6b58L, 
            0x55c0L, 0xab60L, 0x96d5L, 0x92e0L, 0xc960L, 0xd954L, 0xd4a0L, 0xda50L, 0x7552L, 0x56a0L, 0xabb7L, 0x25d0L, 0x92d0L, 0xcab5L, 0xa950L, 0xb4a0L, 
            0xbaa4L, 0xad50L, 0x55d9L, 0x4ba0L, 0xa5b0L, 0x15176L, 0x52b0L, 0xa930L, 0x7954L, 0x6aa0L, 0xad50L, 0x5b52L, 0x4b60L, 0xa6e6L, 0xa4e0L, 0xd260L, 
            0xea65L, 0xd530L, 0x5aa0L, 0x76a3L, 0x96d0L, 0x4bd7L, 0x4ad0L, 0xa4d0L, 0x1d0b6L, 0xd250L, 0xd520L, 0xdd45L, 0xb5a0L, 0x56d0L, 0x55b2L, 0x49b0L, 
            0xa577L, 0xa4b0L, 0xaa50L, 0x1b255L, 0x6d20L, 0xada0L
         };
        private static string[] nStr1 = new string[] { "", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
        private static string[] sFtv = new string[] { 
            "0101 新年元旦", "0202 世界湿地日", "0207 国际声援南非日", "0210 国际气象节", "0214 情人节", "0301 国际海豹日", "0303 全国爱耳日", "0308 国际妇女节", "0312 植树节 孙中山逝世纪念日", "0314 国际警察日", "0315 国际消费者权益日", "0317 中国国医节 国际航海日", "0321 世界森林日 消除种族歧视国际日", "0321 世界儿歌日", "0322 世界水日", "0323 世界气象日", 
            "0324 世界防治结核病日", "0325 全国中小学生安全教育日", "0330 巴勒斯坦国土日", "0401 愚人节 全国爱国卫生运动月(四月) 税收宣传月(四月)", "0407 世界卫生日", "0422 世界地球日", "0423 世界图书和版权日", "0424 亚非新闻工作者日", "0501 国际劳动节", "0504 中国五四青年节", "0505 碘缺乏病防治日", "0508 世界红十字日", "0512 国际护士节", "0515 国际家庭日", "0517 世界电信日", "0518 国际博物馆日", 
            "0520 全国学生营养日", "0523 国际牛奶日", "0531 世界无烟日", "0601 国际儿童节", "0605 世界环境日", "0606 全国爱眼日", "0617 防治荒漠化和干旱日", "0623 国际奥林匹克日", "0625 全国土地日", "0626 国际反毒品日", "0701 中国共产党建党日 世界建筑日", "0702 国际体育记者日", "0707 中国人民抗日战争纪念日", "0711 世界人口日", "0730 非洲妇女日", "0801 中国建军节", 
            "0808 中国男子节(爸爸节)", "0815 日本正式宣布无条件投降日", "0908 国际扫盲日 国际新闻工作者日", "0910 教师节", "0914 世界清洁地球日", "0916 国际臭氧层保护日", "0918 九\x00b7一八事变纪念日", "0920 全国爱牙日", "0927 世界旅游日", "1001 国庆节 世界音乐日 国际老人节", "1001 国际音乐日", "1002 国际和平与民主自由斗争日", "1004 世界动物日", "1008 全国高血压日", "1008 世界视觉日", "1009 世界邮政日 万国邮联日", 
            "1010 辛亥革命纪念日 世界精神卫生日", "1013 世界保健日 国际教师节", "1014 世界标准日", "1015 国际盲人节(白手杖节)", "1016 世界粮食日", "1017 世界消除贫困日", "1022 世界传统医药日", "1024 联合国日 世界发展信息日", "1031 世界勤俭日", "1107 十月社会主义革命纪念日", "1108 中国记者日", "1109 全国消防安全宣传教育日", "1110 世界青年节", "1111 国际科学与和平周(本日所属的一周)", "1112 孙中山诞辰纪念日", "1114 世界糖尿病日", 
            "1117 国际大学生节 世界学生节", "1121 世界问候日 世界电视日", "1129 国际声援巴勒斯坦人民国际日", "1201 世界艾滋病日", "1203 世界残疾人日", "1205 国际经济和社会发展志愿人员日", "1208 国际儿童电视日", "1209 世界足球日", "1210 世界人权日", "1212 西安事变纪念日", "1213 南京大屠杀(1937年)纪念日！紧记血泪史！", "1221 国际篮球日", "1224 平安夜", "1225 圣诞节", "1226 毛主席诞辰", "1229 国际生物多样性日"
         };
        private static string[] solarTerm = new string[] { 
            "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", 
            "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
         };
        private static int[] sTermInfo = new int[] { 
            0, 0x52d8, 0xa5e3, 0xf95c, 0x14d59, 0x1a206, 0x1f763, 0x24d89, 0x2a45d, 0x2fbdf, 0x353d8, 0x3ac35, 0x404af, 0x45d25, 0x4b553, 0x50d19, 
            0x56446, 0x5bac6, 0x61087, 0x6658a, 0x6b9db, 0x70d90, 0x760cc, 0x7b3b6
         };
        private static int[] year19 = new int[] { 0, 3, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 };
        private static int[] year20 = new int[] { 1, 4, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1 };
        private static int[] year2000 = new int[] { 0, 3, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1 };
        private static string[] Zhi = new string[] { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        private static string AnimalsYear(int y)
        {
            return Animals[(y - 4) % 12];
        }

        private static long[] calElement(int y, int m, int d)
        {
            long[] numArray = new long[7];
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            DateTime time = new DateTime(0x76c, 1, 0x1f);
            TimeSpan span = (TimeSpan) (new DateTime(y, m, d) - time);
            long totalDays = (long) span.TotalDays;
            numArray[5] = totalDays + 40L;
            numArray[4] = 14L;
            num = 0x76c;
            while ((num < 0x802) && (totalDays > 0L))
            {
                num2 = lYearDays(num);
                totalDays -= num2;
                numArray[4] += 12L;
                num++;
            }
            if (totalDays < 0L)
            {
                totalDays += num2;
                num--;
                numArray[4] -= 12L;
            }
            numArray[0] = num;
            numArray[3] = num - 0x748;
            num3 = leapMonth(num);
            numArray[6] = 0L;
            num = 1;
            while ((num < 13) && (totalDays > 0L))
            {
                if (((num3 > 0) && (num == (num3 + 1))) && (numArray[6] == 0))
                {
                    num--;
                    numArray[6] = 1L;
                    num2 = leapDays((int) numArray[0]);
                }
                else
                {
                    num2 = monthDays((int) numArray[0], num);
                }
                if ((numArray[6] == 1L) && (num == (num3 + 1)))
                {
                    numArray[6] = 0L;
                }
                totalDays -= num2;
                if (numArray[6] == 0)
                {
                    numArray[4] += 1L;
                }
                num++;
            }
            if (((totalDays == 0) && (num3 > 0)) && (num == (num3 + 1)))
            {
                if (numArray[6] == 1L)
                {
                    numArray[6] = 0L;
                }
                else
                {
                    numArray[6] = 1L;
                    num--;
                    numArray[4] -= 1L;
                }
            }
            if (totalDays < 0L)
            {
                totalDays += num2;
                num--;
                numArray[4] -= 1L;
            }
            numArray[1] = num;
            numArray[2] = totalDays + 1L;
            return numArray;
        }

        private static string cyclical(int y)
        {
            return cyclicalm((y - 0x76c) + 0x24);
        }

        private static string cyclicalm(int num)
        {
            return (Gan[num % 10] + Zhi[num % 12]);
        }

        private static string FormatDate(int m, int d)
        {
            return string.Format("{0:00}{1:00}", m, d);
        }

        public static LFrameWork.Base.CNDate.CNDate getChinaDate(DateTime dt)
        {
            LFrameWork.Base.CNDate.CNDate date = new LFrameWork.Base.CNDate.CNDate();
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            long[] numArray = calElement(year, month, day);
            date.cnIntYear = (int) numArray[0];
            date.cnIntMonth = (int) numArray[1];
            date.cnIntDay = (int) numArray[2];
            date.cnStrYear = cyclical(year);
            date.cnAnm = AnimalsYear(year);
            date.cnStrMonth = nStr1[(int) numArray[1]] + "月";
            date.cnStrDay = getChinaDate((int) numArray[2]);
            string str = dt.ToString("MMdd");
            string str2 = FormatDate(date.cnIntMonth, date.cnIntDay);
            for (int i = 0; i < solarTerm.Length; i++)
            {
                if (sTerm(dt.Year, i).ToString("MMdd").Equals(dt.ToString("MMdd")))
                {
                    date.cnSolarTerm = solarTerm[i];
                    break;
                }
            }
            foreach (string str3 in sFtv)
            {
                if (str3.Substring(0, 4).Equals(str))
                {
                    date.cnFtvs = str3.Substring(4, str3.Length - 4);
                    break;
                }
            }
            foreach (string str4 in lFtv)
            {
                if (str4.Substring(0, 4).Equals(str2))
                {
                    date.cnFtvl = str4.Substring(4, str4.Length - 4);
                    break;
                }
            }
            dt = dt.AddDays(1.0);
            year = dt.Year;
            month = dt.Month;
            day = dt.Day;
            numArray = calElement(year, month, day);
            if (FormatDate((int) numArray[1], (int) numArray[2]).Equals("0101"))
            {
                date.cnFtvl = "除夕";
            }
            return date;
        }

        private static string getChinaDate(int day)
        {
            string str = "";
            if (day == 10)
            {
                return "初十";
            }
            if (day == 20)
            {
                return "二十";
            }
            if (day == 30)
            {
                return "三十";
            }
            switch ((day / 10))
            {
                case 0:
                    str = "初";
                    break;

                case 1:
                    str = "十";
                    break;

                case 2:
                    str = "廿";
                    break;

                case 3:
                    str = "三";
                    break;
            }
            switch ((day % 10))
            {
                case 1:
                    return (str + "一");

                case 2:
                    return (str + "二");

                case 3:
                    return (str + "三");

                case 4:
                    return (str + "四");

                case 5:
                    return (str + "五");

                case 6:
                    return (str + "六");

                case 7:
                    return (str + "七");

                case 8:
                    return (str + "八");

                case 9:
                    return (str + "九");
            }
            return str;
        }

        public static int GetDaysByMonth(int y, int m)
        {
            int[] numArray1 = new int[] { 0x1f, 0, 0x1f, 30, 0x1f, 30, 0x1f, 0x1f, 30, 0x1f, 30, 0x1f };
            numArray1[1] = DateTime.IsLeapYear(y) ? 0x1d : 0x1c;
            return numArray1[m - 1];
        }

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

        private static int leapDays(int y)
        {
            if (leapMonth(y) == 0)
            {
                return 0;
            }
            if ((lunarInfo[y - 0x76c] & 0x10000L) != 0)
            {
                return 30;
            }
            return 0x1d;
        }

        private static int leapMonth(int y)
        {
            return (int) (lunarInfo[y - 0x76c] & 15L);
        }

        private long[] Lunar(int y, int m)
        {
            long[] numArray = new long[7];
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            DateTime time = new DateTime(0xed8, 2, 0x1f);
            TimeSpan span = (TimeSpan) (new DateTime(y + 0x76c, m + 1, 1) - time);
            long totalDays = (long) span.TotalDays;
            if (y < 0x7d0)
            {
                totalDays += year19[m - 1];
            }
            if (y > 0x7d0)
            {
                totalDays += year20[m - 1];
            }
            if (y == 0x7d0)
            {
                totalDays += year2000[m - 1];
            }
            numArray[5] = totalDays + 40L;
            numArray[4] = 14L;
            num = 0x76c;
            while ((num < 0x802) && (totalDays > 0L))
            {
                num2 = lYearDays(num);
                totalDays -= num2;
                numArray[4] += 12L;
                num++;
            }
            if (totalDays < 0L)
            {
                totalDays += num2;
                num--;
                numArray[4] -= 12L;
            }
            numArray[0] = num;
            numArray[3] = num - 0x748;
            num3 = leapMonth(num);
            numArray[6] = 0L;
            num = 1;
            while ((num < 13) && (totalDays > 0L))
            {
                if (((num3 > 0) && (num == (num3 + 1))) && (numArray[6] == 0))
                {
                    num--;
                    numArray[6] = 1L;
                    num2 = leapDays((int) numArray[0]);
                }
                else
                {
                    num2 = monthDays((int) numArray[0], num);
                }
                if ((numArray[6] == 1L) && (num == (num3 + 1)))
                {
                    numArray[6] = 0L;
                }
                totalDays -= num2;
                if (numArray[6] == 0)
                {
                    numArray[4] += 1L;
                }
                num++;
            }
            if (((totalDays == 0) && (num3 > 0)) && (num == (num3 + 1)))
            {
                if (numArray[6] == 1L)
                {
                    numArray[6] = 0L;
                }
                else
                {
                    numArray[6] = 1L;
                    num--;
                    numArray[4] -= 1L;
                }
            }
            if (totalDays < 0L)
            {
                totalDays += num2;
                num--;
                numArray[4] -= 1L;
            }
            numArray[1] = num;
            numArray[2] = totalDays + 1L;
            return numArray;
        }

        private static int lYearDays(int y)
        {
            int num2 = 0x15c;
            for (int i = 0x8000; i > 8; i = i >> 1)
            {
                if ((lunarInfo[y - 0x76c] & i) != 0)
                {
                    num2++;
                }
            }
            return (num2 + leapDays(y));
        }

        private static int monthDays(int y, int m)
        {
            if ((lunarInfo[y - 0x76c] & (((int) 0x10000) >> m)) == 0)
            {
                return 0x1d;
            }
            return 30;
        }

        private static DateTime sTerm(int y, int n)
        {
            double num = 31556925974.7 * (y - 0x76c);
            double num2 = sTermInfo[n];
            DateTime time = new DateTime(0x76c, 1, 6, 2, 5, 0);
            return time.AddMilliseconds(num).AddMinutes(num2);
        }
    }
}

