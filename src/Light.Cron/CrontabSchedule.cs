using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    public class CrontabSchedule
    {
        static readonly char[] Separator = { ' ' };

        static readonly AllContabValue all = new AllContabValue();

        public static CrontabSchedule Parse(string value)
        {
            if (TryParse(value, out CrontabSchedule crontab)) {
                return crontab;
            }
            else {
                throw new ArgumentException("value format error");
            }
        }

        public static bool TryParse(string value, out CrontabSchedule crontab)
        {
            if (string.IsNullOrEmpty(value)) {
                crontab = null;
                return false;
            }
            string[] array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 5) {
                return TryConvertBasicSchedule(array, out crontab);
            }
            else {
                crontab = null;
                return false;
            }
        }

        private static bool TryConvertBasicSchedule(string[] array, out CrontabSchedule crontab)
        {
            crontab = null;
            var m = array[0];
            var h = array[1];
            var d = array[2];
            var M = array[3];
            var w = array[4];
            BasicContabValue minute;
            BasicContabValue hour;
            BasicContabValue day;
            BasicContabValue month;
            BasicContabValue week;

            if (m == "*" || m == "*/1") {
                minute = all;
            }
            else {
                if (MinuteCrontabValue.TryParse(m, out MinuteCrontabValue result)) {
                    minute = result;
                }
                else {
                    return false;
                }
            }
            if (h == "*" || h == "*/1") {
                hour = all;
            }
            else {
                if (HourCrontabValue.TryParse(h, out HourCrontabValue result)) {
                    hour = result;
                }
                else {
                    return false;
                }
            }
            if (d == "*" || d == "*/1") {
                day = all;
            }
            else {
                if (DayCrontabValue.TryParse(d, out DayCrontabValue result)) {
                    day = result;
                }
                else {
                    return false;
                }
            }
            if (M == "*" || M == "*/1") {
                month = all;
            }
            else {
                if (MonthCrontabValue.TryParse(M, out MonthCrontabValue result)) {
                    month = result;
                }
                else {
                    return false;
                }
            }
            if (w == "*" || w == "*/1") {
                week = all;
            }
            else {
                if (WeekCrontabValue.TryParse(w, out WeekCrontabValue result)) {
                    week = result;
                }
                else {
                    return false;
                }
            }
            crontab = new CrontabSchedule(minute, hour, day, month, week);
            return true;
        }

        readonly TimeCrontabValue timeRange;
        readonly BasicContabValue minute;
        readonly BasicContabValue hour;
        readonly BasicContabValue day;
        readonly BasicContabValue month;
        readonly BasicContabValue week;

        CrontabSchedule(BasicContabValue minute, BasicContabValue hour, BasicContabValue day, BasicContabValue month, BasicContabValue week)
        {
            this.minute = minute;
            this.hour = hour;
            this.day = day;
            this.month = month;
            this.week = week;
        }

        public bool Check(DateTime time)
        {
            if (timeRange == null) {
                var m = minute.Check(time);
                var h = hour.Check(time);
                var d = day.Check(time);
                var M = month.Check(time);
                var w = week.Check(time);
                return m & h & M & d & w;
            }
            else {
                var t = timeRange.Check(time);
                var d = day.Check(time);
                var M = month.Check(time);
                var w = week.Check(time);
                return t & M & d & w;
            }
        }
    }
}
