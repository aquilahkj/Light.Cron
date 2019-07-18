using System;
using System.Collections.Generic;
using System.Reflection;

namespace Light.Cron
{
    public class CrontabOptions
    {
        public List<Assembly> Assemblies
        {
            get;
            set;
        }

        public bool ScanAllAssembly
        {
            get;
            set;
        }
    }
}
