using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    class CrontabValueDay
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

        public static bool TryParse(string value, out CrontabValueDay crontabValue)
        {
            crontabValue = null;
            var list = new List<CrontabValueNode>();
            var listDyn = new List<CrontabDynamicEndValueNode>();
            var array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array) {
                if (item == "*" || item == "*/1") {
                    crontabValue = new CrontabValueDay(null, null);
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
                crontabValue = new CrontabValueDay(list, listDyn);
                return true;
            }
        }


        CrontabValueDay(List<CrontabValueNode> list, List<CrontabDynamicEndValueNode> listDyn)
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

        // public override bool AllowNext => true;

        public bool Check(DateTime time, bool useNext, out bool next)
        {
            if (list == null && listDyn == null) {
                next = false;
                return true;
            }
            else {
                if (useNext) {
                    time = time.AddDays(-1);
                }
                var value = time.Day;
                if (listDyn.Count > 0) {
                    var i = GetMonthMaxDay(time);
                    foreach (var item in listDyn) {
                        if (item.Check(i, value, out next)) {
                            return true;
                        }
                    }
                }
                foreach (var item in list) {
                    if (item.Check(value, out next)) {
                        return true;
                    }
                }
                next = false;
                return false;
            }
        }
    }
}
