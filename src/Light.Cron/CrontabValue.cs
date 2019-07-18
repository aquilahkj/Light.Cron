using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    abstract class CrontabValue
    {
        public abstract bool Check(DateTime time);
    }


    class MinuteCrontabValue : CrontabValue
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 0;
        const int max = 59;
        readonly List<CrontabValueNode> list;

        public static bool TryParse(string value, out MinuteCrontabValue crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new MinuteCrontabValue(null);
                    return true;
                }
                if (int.TryParse(item, out int data)) {
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                return false;
            }
            if (list.Count == 0) {
                return false;
            }
            else {
                crontabValue = new MinuteCrontabValue(list);
                return true;
            }
        }

        MinuteCrontabValue(List<CrontabValueNode> list)
        {
            this.list = list;
        }

        public override bool Check(DateTime time)
        {
            if (list == null) {
                return true;
            }
            else {
                var value = time.Minute;
                foreach (var item in list) {
                    if (item.Check(value)) {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    class HourCrontabValue : CrontabValue
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 0;
        const int max = 23;
        readonly List<CrontabValueNode> list;

        public static bool TryParse(string value, out HourCrontabValue crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new HourCrontabValue(null);
                    return true;
                }
                if (int.TryParse(item, out int data)) {
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                return false;
            }
            if (list.Count == 0) {
                return false;
            }
            else {
                crontabValue = new HourCrontabValue(list);
                return true;
            }
        }

        HourCrontabValue(List<CrontabValueNode> list)
        {
            this.list = list;
        }

        public override bool Check(DateTime time)
        {
            if (list == null) {
                return true;
            }
            else {
                var value = time.Hour;
                foreach (var item in list) {
                    if (item.Check(value)) {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    class MonthCrontabValue : CrontabValue
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 1;
        const int max = 12;
        readonly List<CrontabValueNode> list;

        public static bool TryParse(string value, out MonthCrontabValue crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new MonthCrontabValue(null);
                    return true;
                }
                if (int.TryParse(item, out int data)) {
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                return false;
            }
            if (list.Count == 0) {
                return false;
            }
            else {
                crontabValue = new MonthCrontabValue(list);
                return true;
            }
        }

        MonthCrontabValue(List<CrontabValueNode> list)
        {
            this.list = list;
        }

        public override bool Check(DateTime time)
        {
            if (list == null) {
                return true;
            }
            else {
                var value = time.Month;
                foreach (var item in list) {
                    if (item.Check(value)) {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    class WeekCrontabValue : CrontabValue
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 0;
        const int max = 6;
        readonly List<CrontabValueNode> list;

        public static bool TryParse(string value, out WeekCrontabValue crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new WeekCrontabValue(null);
                    return true;
                }
                if (int.TryParse(item, out int data)) {
                    if (data == 7)
                        data = 0;
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    if (start == 7)
                        start = 0;
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    if (end == 7)
                        end = 0;
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                return false;
            }
            if (list.Count == 0) {
                return false;
            }
            else {
                crontabValue = new WeekCrontabValue(list);
                return true;
            }
        }

        WeekCrontabValue(List<CrontabValueNode> list)
        {
            this.list = list;
        }

        public override bool Check(DateTime time)
        {
            if (list == null) {
                return true;
            }
            else {
                var value = (int)time.DayOfWeek;
                foreach (var item in list) {
                    if (item.Check(value)) {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    class DayCrontabValue : CrontabValue
    {
        static readonly char[] Separator = { ',' };

        static readonly Regex EndRegex = new Regex(@"^e(?<end>\d)*$");

        static readonly Regex EndFromToRegex = new Regex(@"^(?<start>\d+|e\d*)-(?<end>e?\d+|e\d*)(/(?<interval>\d+))?$");

        static readonly Regex FromToRegex = new Regex(@"^(?<start>\d+)-(?<end>\d+)(/(?<interval>\d+))?$");

        static readonly Regex AllFromToRegex = new Regex(@"^\*/(?<interval>\d+)$");

        const int min = 1;
        const int max = 31;
        readonly List<CrontabValueNode> list;
        readonly List<CrontabDynamicEndValueNode> listDyn;

        public static bool TryParse(string value, out DayCrontabValue crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var listDyn = new List<CrontabDynamicEndValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new DayCrontabValue(null, null);
                    return true;
                }
                var endmatch = EndRegex.Match(item);
                if (endmatch.Success) {
                    var end = 0;
                    if (endmatch.Groups["end"].Success) {
                        var i = Convert.ToInt32(endmatch.Groups["end"].Value);
                        if (i > max - 1) {
                            return false;
                        }
                        end = -1 * i;
                    }
                    listDyn.Add(new SingleCrontabDynamicValueNode(end));
                    continue;
                }
                if (int.TryParse(item, out int data)) {
                    if (data >= min && data <= max) {
                        list.Add(new SingleCrontabValueNode(data));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var aftmatch = AllFromToRegex.Match(item);
                if (aftmatch.Success) {
                    var interval = Convert.ToInt32(aftmatch.Groups["interval"].Value);
                    if (interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, min, max, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var ftmatch = FromToRegex.Match(item);
                if (ftmatch.Success) {
                    var start = Convert.ToInt32(ftmatch.Groups["start"].Value);
                    var end = Convert.ToInt32(ftmatch.Groups["end"].Value);
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                    }
                    if (start >= min && start <= max && end >= min && end <= max && interval > 0) {
                        list.Add(new RangeCrontabValueNode(min, max, start, end, interval));
                        continue;
                    }
                    else {
                        return false;
                    }
                }
                var eftmatch = EndFromToRegex.Match(item);
                if (eftmatch.Success) {
                    var startv = eftmatch.Groups["start"].Value;
                    var endv = eftmatch.Groups["end"].Value;
                    var start = 0;
                    var end = 0;
                    if (startv[0] == 'e') {
                        if (startv.Length > 1) {
                            var i = Convert.ToInt32(startv.Substring(1));
                            if (i > max - 1) {
                                return false;
                            }
                            start = -1 * i;
                        }
                    }
                    else {
                        start = Convert.ToInt32(startv);
                        if (start == 0 || start > max)
                            return false;
                    }
                    if (endv[0] == 'e') {
                        if (endv.Length > 1) {
                            var i = Convert.ToInt32(endv.Substring(1));
                            if (i > max - 1) {
                                return false;
                            }
                            end = -1 * i;
                        }
                    }
                    else {
                        end = Convert.ToInt32(endv);
                        if (end == 0 || end > max)
                            return false;
                    }
                    var interval = 1;
                    if (ftmatch.Groups["interval"].Success) {
                        interval = Convert.ToInt32(ftmatch.Groups["interval"].Value);
                        if (interval == 0) {
                            return false;
                        }
                    }
                    listDyn.Add(new RangeCrontabDynamicValueNode(min, max, start, end, interval));
                    continue;
                }

                return false;
            }
            if (list.Count == 0 && listDyn.Count == 0) {
                return false;
            }
            else {
                crontabValue = new DayCrontabValue(list, listDyn);
                return true;
            }
        }


        DayCrontabValue(List<CrontabValueNode> list, List<CrontabDynamicEndValueNode> listDyn)
        {
            this.listDyn = listDyn;
            this.list = list;
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

        public override bool Check(DateTime time)
        {
            if (list == null && listDyn==null) {
                return true;
            }
            else {
                var value = time.Day;
                if (listDyn.Count > 0) {
                    var i = GetMonthMaxDay(time);
                    foreach (var item in listDyn) {
                        if (item.Check(i, value)) {
                            return true;
                        }
                    }
                }
                foreach (var item in list) {
                    if (item.Check(value)) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
