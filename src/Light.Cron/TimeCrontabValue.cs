using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    class TimeCrontabValue : CrontabValue
    {
        static readonly Regex regex = new Regex("^(?<fromHour>[0-1]?[0-9]|2[0-3]):(?<fromMinute>[0-5]?[0-9])-(?<toHour>[0-1]?[0-9]|2[0-3]):(?<toMinute>[0-5]?[0-9])(/(?<interval>\\d+))?$", RegexOptions.Compiled);

        static readonly char[] separator = { ',' };

        class TimeRange
        {
            readonly int fromTotalMinute;
            readonly int toTotalMinute;
            readonly int interval;

            public TimeRange(int fromHour, int fromMinute, int toHour, int toMinute, int interval)
            {
                this.fromTotalMinute = fromHour * 60 + fromMinute;
                this.toTotalMinute = toHour * 60 + toMinute;
                this.interval = interval;
            }

            const int MAX_MINUTE = 23 * 60 + 59;


            public bool CheckTimeRange(DateTime time)
            {
                var minute = time.Hour * 60 + time.Minute;
                if (fromTotalMinute <= toTotalMinute) {
                    return minute >= fromTotalMinute && minute <= toTotalMinute && (minute - fromTotalMinute) % interval == 0;
                }
                else {
                    if (minute >= toTotalMinute && minute <= MAX_MINUTE && (minute - toTotalMinute) % interval == 0) {
                        return true;
                    }

                    var fromMinute1 = toTotalMinute;
                    var toMinute1 = fromTotalMinute + MAX_MINUTE + 1;
                    var minute1 = minute + MAX_MINUTE + 1;
                    if (minute1 >= fromMinute1 && minute1 <= toMinute1 && (minute1 - fromMinute1) % interval == 0) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }

        public static bool TryParse(string value, out TimeCrontabValue timeRange)
        {
            timeRange = null;
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            string[] rangearr = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
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
                    var interval = 1;
                    if (match.Groups["interval"].Success) {
                        interval = Convert.ToInt32(match.Groups["interval"].Value);
                    }
                    var timerange = new TimeRange(fromHour, fromMinute, toHour, toMinute, interval);
                    list.Add(timerange);
                }
                else {
                    return false;
                }
            }

            timeRange = new TimeCrontabValue(list);
            return true;
        }

        TimeCrontabValue(List<TimeRange> list)
        {
            this.rangeList = list;
        }

        readonly List<TimeRange> rangeList;

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
