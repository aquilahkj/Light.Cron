using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    public class DateRange
    {
        public int Year { get; private set; }

        public int Month { get; private set; }

        public int Day { get; private set; }

        static Regex TimeRangeRegex = new Regex("^((?<year>\\d+)Y)?((?<month>\\d+)M)?((?<day>\\d+)D)?$", RegexOptions.Compiled);

        public static bool TryParse(string value, out DateRange timeRange)
        {
            if (string.IsNullOrEmpty(value)) {
                timeRange = null;
                return false;
            }
            var match = TimeRangeRegex.Match(value);
            if (match.Success) {
                timeRange = new DateRange();
                if (match.Groups["year"].Success) {
                    var year = Convert.ToInt32(match.Groups["year"].Value);
                    timeRange.Year = year;
                }
                if (match.Groups["month"].Success) {
                    var month = Convert.ToInt32(match.Groups["month"].Value);
                    timeRange.Month = month;
                }
                if (match.Groups["day"].Success) {
                    var day = Convert.ToInt32(match.Groups["day"].Value);
                    timeRange.Day = day;
                }
                return true;
            }
            else {
                timeRange = null;
                return false;
            }
        }

        public DateTime AddTime(DateTime dateTime)
        {
            if (Year > 0) {
                dateTime = dateTime.AddYears(Year);
            }
            if (Month > 0) {
                dateTime = dateTime.AddMonths(Month);
            }
            if (Day > 0) {
                dateTime = dateTime.AddDays(Day);
            }
            return dateTime;
        }

        public DateTime ReduceTime(DateTime dateTime)
        {
            if (Year > 0) {
                dateTime = dateTime.AddYears(-1 * Year);
            }
            if (Month > 0) {
                dateTime = dateTime.AddMonths(-1 * Month);
            }
            if (Day > 0) {
                dateTime = dateTime.AddDays(-1 * Day);
            }
            return dateTime;
        }
    }
}
