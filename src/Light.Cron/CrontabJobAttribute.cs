using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    /// <summary>
    /// Specified the class for Crontabjob
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CrontabJobAttribute : Attribute
    {
       
    }
}
