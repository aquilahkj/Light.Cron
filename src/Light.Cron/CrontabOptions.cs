using System;
using System.Collections.Generic;
using System.Reflection;

namespace Light.Cron
{
    /// <summary>
    /// 
    /// </summary>
    public class CrontabOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Assembly> Assemblies
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ScanAllAssembly
        {
            get;
            set;
        }
    }
}
