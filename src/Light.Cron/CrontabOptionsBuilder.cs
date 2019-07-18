using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Light.Cron
{
    public class CrontabOptionsBuilder
    {
        readonly List<Assembly> assemblies = new List<Assembly>();

        public void SetAssembly(Assembly assembly)
        {
            if (!assemblies.Contains(assembly)) {
                assemblies.Add(assembly);
            }
        }

        public void SetAssembly(string assemblyString)
        {
            var assembly = Assembly.Load(assemblyString);
            if (!assemblies.Contains(assembly)) {
                assemblies.Add(assembly);
            }
        }

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
