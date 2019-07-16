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
        readonly IServiceProvider services;

        readonly ILogger logger;

        readonly Dictionary<string, CrontabExecutor> executorDict;

        readonly CancellationTokenSource cts = new CancellationTokenSource();


        public CrontabService(IServiceProvider services)
        {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }
            this.services = services;
            this.logger = services.GetService<ILogger>();
            executorDict = new Dictionary<string, CrontabExecutor>();
            var list = CreateCrontabSettings();
            foreach (var item in list) {
                executorDict.Add(item.Identifier, item);
            }

            var dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            var next = dt.AddMinutes(1);
            var due = Convert.ToInt32(Math.Ceiling((next - DateTime.Now).TotalMilliseconds));

            List<CrontabExecutor> enablelist;
            lock (executorDict) {
                enablelist = executorDict.Values.ToList();
            }
            foreach (var item in enablelist) {
                Task.Factory.StartNew(() => {
                    item.Run(services, dt, true);
                });
            }

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

            List<CrontabExecutor> enablelist;
            lock (executorDict) {
                enablelist = executorDict.Values.ToList();
            }
            foreach (var item in enablelist) {
                Task.Factory.StartNew(() => {
                    item.Run(services, dt, false);
                });
            }
        }

        //public void SetCrontab(string name, string scheduleValue, bool skipWhileExecuting, bool runImmediately)
        //{
        //    var typeName = name.Substring(0, name.LastIndexOf('.'));
        //    var methodName = name.Substring(name.LastIndexOf('.') + 1);
        //    var type = Type.GetType(typeName, true).GetTypeInfo();
        //    var method = type.GetMethod(methodName);


        //    if (!CrontabSchedule.TryParse(scheduleValue, out CrontabSchedule schedule)) {
        //        throw new CustomAttributeFormatException("Crontab Schedule Format Error");
        //    }
        //    var item = new CrontabExecutor(type, method, schedule, skipWhileExecuting, runImmediately, false);
        //    lock (executorDict) {
        //        executorDict[item.Identifier] = item;
        //    }
        //}

        public void Disable(string name)
        {
            //lock (executorDict) {
            if (executorDict.TryGetValue(name, out CrontabExecutor executor)) {
                executor.Enable = false;
            }
            else {
                throw new ArgumentException($"Method Name {name} dose not exists");
            }
            //}
        }

        public void Enable(string name)
        {
            //lock (executorDict) {
            if (executorDict.TryGetValue(name, out CrontabExecutor executor)) {
                executor.Enable = true;
            }
            else {
                throw new ArgumentException($"Method Name {name} dose not exists");
            }
            //}
        }

        private List<CrontabExecutor> CreateCrontabSettings()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var dependencyContext = DependencyContext.Load(entryAssembly);
            IList<Assembly> assemblys;
            if (dependencyContext == null) {
                assemblys = new[] { entryAssembly };
            }
            else {
                var assName = Assembly.GetExecutingAssembly().GetName().Name;
                assemblys = dependencyContext
                 .RuntimeLibraries
                 .Where(l => l.Dependencies.Any(d => string.Equals(assName, d.Name, StringComparison.Ordinal)))
                 .SelectMany(l => l.GetDefaultAssemblyNames(dependencyContext))
                 .Select(assembly => Assembly.Load(new AssemblyName(assembly.Name)))
                 .ToArray();
            }
            var types = assemblys.SelectMany(a => a.DefinedTypes.Where(y => y.GetCustomAttribute<CrontabJobAttribute>() != null))
                                 .ToArray();
            var list = new List<CrontabExecutor>();
            foreach (var type in types) {
                foreach (var method in type.DeclaredMethods) {
                    var attribute = method.GetCustomAttribute<CrontabScheduleAttribute>();
                    if (attribute != null) {
                        var identify = type.FullName + '.' + method.Name;
                        var arr = attribute.Schedule.Split('|');
                        if (arr.Length == 0) {
                            throw new CustomAttributeFormatException("Crontab Schedule No Data");
                        }
                        var schedules = new CrontabSchedule[arr.Length];
                        for (int i = 0; i < arr.Length; i++) {
                            if (CrontabSchedule.TryParse(arr[i], out CrontabSchedule schedule)) {
                                schedules[i] = schedule;
                            }
                            else {
                                throw new CustomAttributeFormatException("Crontab Schedule Format Error");
                            }
                        }
                        var item = new CrontabExecutor(type, method, schedules, attribute.SkipWhileExecuting, attribute.RunImmediately);
                        item.SkipWhileExecuting = attribute.SkipWhileExecuting;
                        item.RunImmediately = attribute.RunImmediately;
                        list.Add(item);
                    }
                }
            }
            return list;

        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    //this.timer.Dispose();
                    cts.Cancel();
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
