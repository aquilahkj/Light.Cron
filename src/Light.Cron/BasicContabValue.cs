using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    abstract class BasicContabValue
    {
        public abstract bool Check(DateTime time);
    }

    class AllContabValue : BasicContabValue
    {
        public override bool Check(DateTime time)
        {
            return true;
        }
    }

    abstract class NormalCrontabValue : BasicContabValue
    {
        readonly protected List<int> list;

        readonly protected bool containEnd;

        protected NormalCrontabValue(List<int> list)
        {
            this.list = list;
            if (this.list.Contains(-1)) {
                containEnd = true;
                list.Remove(-1);
            }
        }


        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex("^(?<from>\\d+|e)-(?<to>\\d+|e)$");

        static bool TryParseFromTo(string value, int min, int max, int interval, out List<int> list)
        {
            list = null;
            var ftmatch = FromToRegex.Match(value);
            if (ftmatch.Success) {
                var first = ftmatch.Groups["from"].Value;
                var second = ftmatch.Groups["to"].Value;
                bool end = false;
                int from;
                if (first == "e") {
                    end = true;
                    from = max - 1;
                }
                else {
                    from = Convert.ToInt32(first);
                }
                int to;
                if (second == "e") {
                    end = true;
                    to = max - 1;
                }
                else {
                    to = Convert.ToInt32(second);
                }
                if (from < min || from >= max || to < min || to >= max) {
                    return false;
                }
                if (interval > (from <= to ? to + 1 - from : max - from + to + 1)) {
                    return false;
                }
                list = new List<int>();
                if (end) {
                    list.Add(-1);
                }
                if (from <= to) {
                    var i = from;
                    do {
                        list.Add(i);
                        i += interval;
                    } while (i <= to);
                    return true;
                }
                else {
                    var i = from;
                    do {
                        list.Add(i);
                        i += interval;
                    } while (i < max);
                    i -= max - min;
                    while (i <= to) {
                        list.Add(i);
                        i += interval;
                    }
                    return true;
                }
            }
            else {
                return false;
            }
        }

        static bool TryParseCollection(string value, int min, int max, out List<int> list)
        {
            list = null;
            var arr = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0) {
                return false;
            }
            var list1 = new List<int>();
            foreach (var item in arr) {
                if (item == "e") {
                    list1.Add(-1);
                }
                else if (item.Contains("-")) {
                    if (TryParseFromTo(item, min, max, 1, out List<int> inlist)) {
                        foreach (int i in inlist) {
                            if (!list1.Contains(i)) {
                                list1.Add(i);
                            }
                        }
                    }
                    else {
                        return false;
                    }
                }
                else if (int.TryParse(item, out int v) && v >= min && v < max) {
                    if (!list1.Contains(v)) {
                        list1.Add(v);
                    }
                }
                else {
                    return false;
                }
            }
            list = list1;
            return true;
        }

        protected static bool TryParseValue(string value, int min, int max, out List<int> list)
        {
            max++;
            list = null;
            int index = value.IndexOf('/');
            if (index > -1) {
                if (index > 0 && index < value.Length - 1) {
                    var first = value.Substring(0, index);
                    var second = value.Substring(index + 1);
                    if (int.TryParse(second, out int interval)) {
                        if (interval <= 0) {
                            return false;
                        }
                        else {
                            if (first == "*") {
                                if (interval > max - min) {
                                    return false;
                                }
                                list = new List<int>();
                                var i = min;
                                do {
                                    list.Add(i);
                                    i += interval;
                                } while (i < max);
                                return true;
                            }
                            if (first.Contains("-")) {
                                return TryParseFromTo(first, min, max, interval, out list);
                            }
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }
            else if (value.Contains(",")) {
                return TryParseCollection(value, min, max, out list);
            }
            else if (value.Contains("-")) {
                return TryParseFromTo(value, min, max, 1, out list);
            }
            else if (int.TryParse(value, out int num)) {
                if (num < min || num >= max) {
                    return false;
                }
                list = new List<int> {
                        num
                    };
                return true;
            }
            return false;
        }


    }

    class MinuteCrontabValue : NormalCrontabValue
    {
        public static bool TryParse(string value, out MinuteCrontabValue minute)
        {
            if (TryParseValue(value, 0, 59, out List<int> mlist)) {
                minute = new MinuteCrontabValue(mlist);
                return true;
            }
            else {
                minute = null;
                return false;
            }
        }

        MinuteCrontabValue(List<int> list) : base(list)
        {

        }

        public override bool Check(DateTime time)
        {
            var cur = time.Minute;
            if (containEnd && cur == 59) {
                return true;
            }
            else {
                return list.Contains(cur);
            }
        }
    }

    class HourCrontabValue : NormalCrontabValue
    {
        public static bool TryParse(string value, out HourCrontabValue hour)
        {
            if (TryParseValue(value, 0, 23, out List<int> mlist)) {
                hour = new HourCrontabValue(mlist);
                return true;
            }
            else {
                hour = null;
                return false;
            }
        }

        HourCrontabValue(List<int> list) : base(list)
        {

        }

        public override bool Check(DateTime time)
        {
            var cur = time.Hour;
            if (containEnd && cur == 23) {
                return true;
            }
            else {
                return list.Contains(cur);
            }
        }
    }

    class DayCrontabValue : NormalCrontabValue
    {
        public static bool TryParse(string value, out DayCrontabValue day)
        {
            if (TryParseValue(value, 1, 31, out List<int> mlist)) {
                day = new DayCrontabValue(mlist);
                return true;
            }
            else {
                day = null;
                return false;
            }
        }

        DayCrontabValue(List<int> list) : base(list)
        {

        }

        public override bool Check(DateTime time)
        {
            var cur = time.Day;
            if (containEnd && GetMonthMaxDay(time) == cur) {
                return true;
            }
            else {
                return list.Contains(cur);
            }
        }

        private int GetMonthMaxDay(DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;
            int day;
            if (month == 2) {
                if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))//判断是不是闰年    
                {
                    day = 29;
                }
                else {
                    day = 28;
                }
            }
            else {
                switch (month) {
                    case 04:
                    case 06:
                    case 09:
                    case 11: day = 30; break;
                    default: day = 31; break;
                }
            }
            return day;
        }
    }

    class MonthCrontabValue : NormalCrontabValue
    {
        public static bool TryParse(string value, out MonthCrontabValue month)
        {
            if (TryParseValue(value, 1, 12, out List<int> mlist)) {
                month = new MonthCrontabValue(mlist);
                return true;
            }
            else {
                month = null;
                return false;
            }
        }

        MonthCrontabValue(List<int> list) : base(list)
        {

        }

        public override bool Check(DateTime time)
        {
            var cur = time.Month;
            if (containEnd && cur == 12) {
                return true;
            }
            else {
                return list.Contains(cur);
            }
        }
    }

    class WeekCrontabValue : NormalCrontabValue
    {
        public static bool TryParse(string value, out WeekCrontabValue week)
        {
            if (TryParseValue(value, 0, 7, out List<int> mlist)) {
                week = new WeekCrontabValue(mlist);
                return true;
            }
            else {
                week = null;
                return false;
            }
        }

        WeekCrontabValue(List<int> list) : base(list)
        {

        }

        public override bool Check(DateTime time)
        {
            var week = time.DayOfWeek;
            if (week == DayOfWeek.Sunday) {
                if (containEnd) {
                    return true;
                }
                else {
                    return list.Contains(0) || list.Contains(7);
                }
            }
            else {
                return list.Contains((int)week);
            }
        }


    }
}
