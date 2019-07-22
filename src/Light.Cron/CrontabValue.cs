using System;
using System.Text;

namespace Light.Cron
{
    abstract class CrontabValueTimeGroup
    {
        public abstract bool Check(DateTime time, out bool next);
    }

    abstract class CrontabValueDateGroup
    {
        public abstract bool Check(DateTime time, bool useNext);
    }

    class CrontabValueHourMinuteGroup : CrontabValueTimeGroup
    {
        private readonly CrontabValueMinute minute;
        private readonly CrontabValueHour hour;

        public CrontabValueHourMinuteGroup(CrontabValueMinute minute, CrontabValueHour hour)
        {
            this.minute = minute;
            this.hour = hour;
        }

        public override bool Check(DateTime time, out bool next)
        {
            bool useNext = false;
            next = false;
            if (minute != null) {
                if (!minute.Check(time, out next)) {
                    return false;
                }
                useNext = next;
            }
            if (hour != null) {
                if (!hour.Check(time, useNext, out next)) {
                    return false;
                }
            }
            return true;
        }
    }

    class CrontabValueTimeRangeGroup : CrontabValueTimeGroup
    {
        private readonly CrontabValueTimeRange timeRange;

        public CrontabValueTimeRangeGroup(CrontabValueTimeRange timeRange)
        {
            this.timeRange = timeRange;
        }

        public override bool Check(DateTime time, out bool next)
        {
            next = false;
            if (timeRange != null) {
                if (!timeRange.Check(time, out next)) {
                    return false;
                }
            }
            return true;
        }
    }

    class CrontabValueMonthDayGroup : CrontabValueDateGroup
    {
        private readonly CrontabValueDay day;
        private readonly CrontabValueMonth month;

        public CrontabValueMonthDayGroup(CrontabValueDay day, CrontabValueMonth month)
        {
            this.day = day;
            this.month = month;
        }

        public override bool Check(DateTime time, bool useNext)
        {
            if (day != null) {
                if (!day.Check(time, useNext, out bool next)) {
                    return false;
                }
                useNext = next;
            }
            else {
                useNext = false;
            }
            if (month != null) {
                if (!month.Check(time, useNext)) {
                    return false;
                }
            }
            return true;
        }
    }

}