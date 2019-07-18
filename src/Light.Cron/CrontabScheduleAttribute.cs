using System;

namespace Light.Cron
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CrontabScheduleAttribute : Attribute
    {
        public string Schedule { get; }

        public string Name { get;  }

        public CrontabScheduleAttribute(string name, string schedule)
        {
            Name = name;
            Schedule = schedule;
        }

        public bool AllowConcurrentExecution { get; set; } = false;

        public bool RunImmediately { get; set; } = false;

        public bool AutoEnable { get; set; } = true;
    }
}