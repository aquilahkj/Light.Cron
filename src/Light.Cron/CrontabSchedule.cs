using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    /// <summary>
    /// The Crontab Schedule
    /// </summary>
    public class CrontabSchedule
    {
        static readonly char[] Separator = { ' ' };

        /// <summary>
        /// Try to parse the value for the schedule
        /// </summary>
        /// <param name="value"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out CrontabSchedule schedule)
        {
            schedule = null;
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            string[] array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 0) {
                return false;
            }
            var v1 = array[0];
            if (CrontabValueMinute.TryParse(v1, out CrontabValueMinute minute)) {
                CrontabValueHour hour = null;
                CrontabValueDay day = null;
                CrontabValueMonth month = null;
                CrontabValueWeek week = null;
                if (array.Length >= 2) {
                    if (!CrontabValueHour.TryParse(array[1], out hour)) {
                        return false;
                    }
                }
                if (array.Length >= 3) {
                    if (!CrontabValueDay.TryParse(array[2], out day)) {
                        return false;
                    }
                }
                if (array.Length >= 4) {
                    if (!CrontabValueMonth.TryParse(array[3], out month)) {
                        return false;
                    }
                }
                if (array.Length >= 5) {
                    if (!CrontabValueWeek.TryParse(array[4], out week)) {
                        return false;
                    }
                }
                if (array.Length >= 6) {
                    return false;
                }
                var timeGroup = new CrontabValueHourMinuteGroup(minute, hour);
                var dateGroup = new CrontabValueMonthDayGroup(day, month);
                schedule = new CrontabSchedule(value, timeGroup, dateGroup, week);
                return true;
            }
            else if (CrontabValueTimeRange.TryParse(v1, out CrontabValueTimeRange timeRange)) {
                CrontabValueDay day = null;
                CrontabValueMonth month = null;
                CrontabValueWeek week = null;
                if (array.Length >= 2) {
                    if (!CrontabValueDay.TryParse(array[1], out day)) {
                        return false;
                    }
                }
                if (array.Length >= 3) {
                    if (!CrontabValueMonth.TryParse(array[2], out month)) {
                        return false;
                    }
                }
                if (array.Length >= 4) {
                    if (!CrontabValueWeek.TryParse(array[3], out week)) {
                        return false;
                    }
                }
                if (array.Length >= 5) {
                    return false;
                }
                var timeGroup = new CrontabValueTimeRangeGroup(timeRange);
                var dateGroup = new CrontabValueMonthDayGroup(day, month);
                schedule = new CrontabSchedule(value, timeGroup, dateGroup, week);
                return true;
            }
            else {
                return false;
            }
        }

       



        /// <summary>
        /// The schedule value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Check the time in the schedule
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Check(DateTime time)
        {
            if (!timeGroup.Check(time, out bool next)) {
                return false;
            }
            if (!dateGroup.Check(time, next)) {
                return false;
            }
            if (week != null) {
                if (!week.Check(time, next)) {
                    return false;
                }
            }
            return true;
        }

        private readonly CrontabValueTimeGroup timeGroup;
        private readonly CrontabValueDateGroup dateGroup;
        private readonly CrontabValueWeek week;

        CrontabSchedule(string value, CrontabValueTimeGroup timeGroup, CrontabValueDateGroup dateGroup, CrontabValueWeek week)
        {
            this.timeGroup = timeGroup;
            this.dateGroup = dateGroup;
            this.week = week;
            this.Value = value;
        }
    }
}
