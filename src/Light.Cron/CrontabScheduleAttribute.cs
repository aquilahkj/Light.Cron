using System;

namespace Light.Cron
{
    /// <summary>
    /// Specified the method for Crontab Schedule
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CrontabScheduleAttribute : Attribute
    {
        /// <summary>
        /// The schedule value
        /// </summary>
        public string Schedule { get; }

        /// <summary>
        /// The schedule name
        /// </summary>
        public string Name { get;  }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="schedule"></param>
        public CrontabScheduleAttribute(string name, string schedule)
        {
            Name = name;
            Schedule = schedule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        public CrontabScheduleAttribute(string schedule)
        {
            Schedule = schedule;
        }

        /// <summary>
        /// Whether or not allow concurrent execution
        /// </summary>
        public bool AllowConcurrentExecution { get; set; } = false;

        /// <summary>
        /// Whether or not the sechedule is set to enabled, it runs immediately, default is false
        /// </summary>
        public bool RunImmediately { get; set; } = false;

        /// <summary>
        /// Whether or not the service starts, the sechedule is set to enabled, default is true
        /// </summary>
        public bool AutoEnable { get; set; } = true;
    }
}