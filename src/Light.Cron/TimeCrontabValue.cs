using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    class TimeCrontabValue : BasicContabValue
    {
        static readonly Regex regex = new Regex(@"^(?<fromHour>[0-1]?[0-9]|2[0-3]):(?<fromMinute>[0-5]?[0-9])-(?<toHour>[0-1]?[0-9]|2[0-3]):(?<toMinute>[0-5]?[0-9])$", RegexOptions.Compiled);

        static readonly char[] separator = { ';' };

        class TimeRange
        {
            readonly int fromHour;
            readonly int fromMinute;
            readonly int toHour;
            readonly int toMinute;
            readonly int interval;
            readonly HashSet<string> hash = new HashSet<string>();

            public TimeRange(int fromHour, int fromMinute, int toHour, int toMinute, int interval)
            {
                this.fromHour = fromHour;
                this.fromMinute = fromMinute;
                this.toHour = toHour;
                this.toMinute = toMinute;
                this.interval = interval;

                if (interval > 1) {
                    var time = DateTime.Now;
                    var date = time.Date;
                    var from = date.AddHours(fromHour).AddMinutes(fromMinute);
                    var to = date.AddHours(toHour).AddMinutes(toMinute);

                    while (from <= to) {
                        from.AddMinutes(interval);
                        hash.Add(from.ToString("HHmm"));
                    }

                }
            }

            public bool CheckTimeRange(DateTime time)
            {
                if (interval == 1) {
                    var hour = time.Hour;
                    var min = time.Minute;
                    if (hour == fromHour) {
                        if (fromHour == toHour) {
                            return min >= fromMinute && min <= toMinute;
                        }
                        else {
                            return min >= fromMinute;
                        }
                    }
                    else if (hour == toHour) {
                        return min <= toMinute;
                    }
                    else {
                        return hour > fromHour && hour < toHour;
                    }
                }
                else {
                    return hash.Contains(time.ToString("HHmm"));
                }
            }
        }

        public static TimeCrontabValue Parse(string value)
        {
            if (TryParse(value, out TimeCrontabValue timeRange)) {
                return timeRange;
            }
            else {
                throw new ArgumentException($"{nameof(value)} format error");
            }
        }

        public static bool TryParse(string value, out TimeCrontabValue timeRange)
        {
            timeRange = null;
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            int index = value.IndexOf('/');
            string range;
            int interval = 1;
            if (index > -1) {
                if (index > 0 && index < value.Length - 1) {
                    var first = value.Substring(0, index);
                    var second = value.Substring(index + 1);
                    if (int.TryParse(second, out int i)) {
                        range = first;
                        if (i <= 0) {
                            return false;
                        }
                        else {
                            interval = i;
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
            else {
                range = value;
            }
            string[] rangearr = range.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (rangearr.Length == 0) {
                return false;
            }
            List<TimeRange> list = new List<TimeRange>();
            foreach (var item in rangearr) {
                var match = regex.Match(item);
                if (match.Success) {
                    var fromHour = Convert.ToInt32(match.Groups["fromHour"].Value);
                    var fromMinute = Convert.ToInt32(match.Groups["fromMinute"].Value);
                    var toHour = Convert.ToInt32(match.Groups["toHour"].Value);
                    var toMinute = Convert.ToInt32(match.Groups["toMinute"].Value);

                    if (fromHour > toHour) {
                        return false;
                    }
                    else if (fromHour == toHour && fromMinute > toMinute) {
                        return false;
                    }
                    var timerange = new TimeRange(fromHour, fromMinute, toHour, toMinute, interval);
                    list.Add(timerange);
                }
                else {
                    return false;
                }
            }

            timeRange = new TimeCrontabValue(list, interval);
            return true;
        }

        TimeCrontabValue(List<TimeRange> list, int interval)
        {
            this.rangeList = list;
            this.interval = interval;
        }

        public TimeCrontabValue()
        {
        }

        readonly List<TimeRange> rangeList;

        readonly int interval;

        public override bool Check(DateTime time)
        {
            foreach (var item in rangeList) {
                if (item.CheckTimeRange(time)) {
                    return true;
                }
            }
            return false;
        }
    }
}
