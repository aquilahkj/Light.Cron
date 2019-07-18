using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    public class CrontabSchedule
    {
        static readonly char[] Separator = { ' ' };

        public static bool TryParse(string value, out CrontabSchedule crontab)
        {
            crontab = null;
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            string[] array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 0) {
                return false;
            }
            var list = new List<CrontabValue>();
            var v1 = array[0];
            if (MinuteCrontabValue.TryParse(v1, out MinuteCrontabValue minute)) {
                list.Add(minute);
                if (array.Length >= 2) {
                    if (HourCrontabValue.TryParse(array[1], out HourCrontabValue hour)) {
                        list.Add(hour);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 3) {
                    if (DayCrontabValue.TryParse(array[2], out DayCrontabValue day)) {
                        list.Add(day);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 4) {
                    if (MonthCrontabValue.TryParse(array[3], out MonthCrontabValue month)) {
                        list.Add(month);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 5) {
                    if (WeekCrontabValue.TryParse(array[4], out WeekCrontabValue week)) {
                        list.Add(week);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 6) {
                    return false;
                }
                crontab = new CrontabSchedule(list, value);
                return true;
            }
            else if (TimeCrontabValue.TryParse(v1, out TimeCrontabValue timerange)) {
                list.Add(timerange);
                if (array.Length >= 2) {
                    if (DayCrontabValue.TryParse(array[1], out DayCrontabValue day)) {
                        list.Add(day);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 3) {
                    if (MonthCrontabValue.TryParse(array[2], out MonthCrontabValue month)) {
                        list.Add(month);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 4) {
                    if (WeekCrontabValue.TryParse(array[3], out WeekCrontabValue week)) {
                        list.Add(week);
                    }
                    else {
                        return false;
                    }
                }
                if (array.Length >= 5) {
                    return false;
                }
                crontab = new CrontabSchedule(list, value);
                return true;
            }
            else {
                return false;
            }
        }

        readonly List<CrontabValue> list;

        public string Value { get; }

        CrontabSchedule(List<CrontabValue> list, string value)
        {
            this.Value = value;
            this.list = list;
        }

        public bool Check(DateTime time)
        {
            foreach (var item in list) {
                if (!item.Check(time)) {
                    return false;
                }
            }
            return true;
        }
    }
}
