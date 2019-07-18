using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Light.Cron
{
    public class CrontabService : IDisposable
    {
        readonly ILogger logger;

        readonly Dictionary<string, CrontabExecutor> executorDict = new Dictionary<string, CrontabExecutor>();

        readonly CancellationTokenSource cts = new CancellationTokenSource();

        public CrontabService(IServiceProvider services, CrontabOptions options)
        {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }
            var types = GetTypeInfos(options);
            var list = new List<CrontabExecutor>();
            foreach (var type in types) {
                foreach (var method in type.DeclaredMethods) {
                    var attribute = method.GetCustomAttribute<CrontabScheduleAttribute>();
                    if (attribute != null) {
                        if (string.IsNullOrEmpty(attribute.Name)) {
                            throw new CustomAttributeFormatException("Crontab name is empty");
                        }
                        var arr = attribute.Schedule.Split('|');
                        if (arr.Length == 0) {
                            throw new CustomAttributeFormatException($"Crontab '{attribute.Name}' no schedule");
                        }
                        var schedules = new CrontabSchedule[arr.Length];
                        for (int i = 0; i < arr.Length; i++) {
                            if (CrontabSchedule.TryParse(arr[i], out CrontabSchedule schedule)) {
                                schedules[i] = schedule;
                            }
                            else {
                                throw new CustomAttributeFormatException($"Crontab schedule '{arr[i]}' format error");
                            }
                        }
                        var item = new CrontabExecutor(attribute.Name, services, type, method, schedules, attribute.AllowConcurrentExecution, attribute.RunImmediately);
                        if (!executorDict.ContainsKey(item.Name)) {
                            executorDict.Add(item.Name, item);
                            if (attribute.AutoEnable) {
                                item.Enable();
                            }
                        }
                        else {
                            throw new CustomAttributeFormatException($"Crontab '{item.Name}' name is duplicate");
                        }
                    }
                }
            }


            this.logger = services.GetService<ILogger>();

            var dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            var next = dt.AddMinutes(1);
            var due = Convert.ToInt32(Math.Ceiling((next - DateTime.Now).TotalMilliseconds));

            Task.Factory.StartNew(async () => {
                try {
                    await Task.Delay(due, cts.Token);
                    Execute();
                }
                catch (Exception ex) {
                    if (logger != null)
                        logger.LogError(ex, "Execute error");
                }
            }, cts.Token);
        }

        private void Execute()
        {
            if (cts.IsCancellationRequested) {
                return;
            }
            var dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            var next = dt.AddMinutes(1);
            var due = Convert.ToInt32(Math.Ceiling((next - DateTime.Now).TotalMilliseconds));
            Task.Factory.StartNew(async () => {
                try {
                    await Task.Delay(due, cts.Token);
                    Execute();
                }
                catch (Exception ex) {
                    if (logger != null)
                        logger.LogError(ex, "Execute error");
                }
            }, cts.Token);
            var list = executorDict.Values.ToList();
            foreach (var item in list) {
                Task.Factory.StartNew(() => {
                    item.Run();
                });
            }
        }

        public bool Disable(string name)
        {
            if (executorDict.TryGetValue(name, out CrontabExecutor executor)) {
                return executor.Disable();
            }
            else {
                throw new ArgumentException($"Method Name {name} dose not exists");
            }
        }

        public bool Enable(string name)
        {
            if (executorDict.TryGetValue(name, out CrontabExecutor executor)) {
                return executor.Enable();
            }
            else {
                throw new ArgumentException($"Method Name {name} dose not exists");
            }
        }

        private List<TypeInfo> GetTypeInfos(CrontabOptions options)
        {
            HashSet<Assembly> assemblys = new HashSet<Assembly>();
            if (options.ScanAllAssembly) {
                var entryAssembly = Assembly.GetEntryAssembly();
                var dependencyContext = DependencyContext.Load(entryAssembly);
                if (dependencyContext == null) {
                    assemblys.Add(entryAssembly);
                }
                else {
                    var assName = Assembly.GetExecutingAssembly().GetName().Name;
                    var libs = dependencyContext.RuntimeLibraries.Where(lib => lib.Dependencies.Any(dep => string.Equals(assName, dep.Name, StringComparison.Ordinal)));
                    var assNames = libs.SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext));
                    foreach (var name in assNames) {
                        var assembly = Assembly.Load(new AssemblyName(name.Name));
                        assemblys.Add(assembly);
                    }
                }
            }
            if (options.Assemblies != null && options.Assemblies.Count > 0) {
                foreach (var assembly in options.Assemblies) {
                    assemblys.Add(assembly);
                }
            }
            if (assemblys.Count == 0) {
                assemblys.Add(Assembly.GetEntryAssembly());
            }

            var types = assemblys.SelectMany(a => a.DefinedTypes.Where(y => y.GetCustomAttribute<CrontabJobAttribute>() != null)).ToList();
            return types;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    var list = executorDict.Values.ToList();
                    list.ForEach(x => x.Disable());
                    cts.Cancel();
                    cts.Dispose();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CrontabService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }
}
