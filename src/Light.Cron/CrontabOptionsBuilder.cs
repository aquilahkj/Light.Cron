using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Light.Cron
{
    /// <summary>
    /// The crontab options builder
    /// </summary>
    public class CrontabOptionsBuilder
    {
        readonly List<Assembly> assemblies = new List<Assembly>();

        /// <summary>
        /// Set specific assembly to scan crontab object
        /// </summary>
        /// <param name="assembly"></param>
        public void SetAssembly(Assembly assembly)
        {
            if (!assemblies.Contains(assembly)) {
                assemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Set specific assembly to scan crontab object
        /// </summary>
        /// <param name="assemblyString"></param>
        public void SetAssembly(string assemblyString)
        {
            var assembly = Assembly.Load(assemblyString);
            if (!assemblies.Contains(assembly)) {
                assemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Scan all assembly
        /// </summary>
        public bool ScanAllAssembly
        {
            get;
            set;
        }

        internal void BuildServices(IServiceCollection services)
        {
            var options = new CrontabOptions() {
                Assemblies = assemblies,
                ScanAllAssembly = ScanAllAssembly
            };
            services.AddSingleton(options); 
            services.AddSingleton<CrontabService>();
        }

    }
}
