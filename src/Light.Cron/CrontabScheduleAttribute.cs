using System;

namespace Light.Cron
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CrontabScheduleAttribute : Attribute
    {
        public string Schedule { get; private set; }

        public CrontabScheduleAttribute(string schedule)
        {
            Schedule = schedule;
        }

        public bool SkipWhileExecuting { get; set; } = false;

        public bool RunImmediately { get; set; } = false;
    }
}