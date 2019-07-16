using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Light.Cron
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CrontabJobAttribute : Attribute
    {
       
    }
}
