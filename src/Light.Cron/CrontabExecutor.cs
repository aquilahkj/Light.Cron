using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Light.Cron
{
    /// <summary>
    /// The Crontab Executor
    /// </summary>
    public class CrontabExecutor
    {
        private readonly IServiceProvider services;
        private readonly CancellationToken cancellationToken;
        private readonly CrontabSchedule[] schedule;
        private readonly TypeInfo type;
        private readonly MethodInfo method;

        internal CrontabExecutor(string name, IServiceProvider services, CancellationToken cancellationToken, TypeInfo typeInfo, MethodInfo method, CrontabSchedule[] schedule, bool allowConcurrentExecution, bool runImmediately)
        {
            this.services = services;
            this.cancellationToken = cancellationToken;
            Name = name;
            this.method = method;
            type = typeInfo;
            this.schedule = schedule;
            AllowConcurrentExecution = allowConcurrentExecution;
            RunImmediately = runImmediately;
        }

        int flag;

        /// <summary>
        /// The name of the crontab executor
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The object type name of the crontab executor
        /// </summary>
        public string TypeName
        {
            get {
                return type.FullName;
            }
        }

        /// <summary>
        /// The method name of the crontab executor
        /// </summary>
        public string MethodName
        {
            get {
                return method.Name;
            }
        }

        /// <summary>
        /// Whether or not allow concurrent execution
        /// </summary>
        public bool AllowConcurrentExecution { get; }

        /// <summary>
        /// Whether or not the sechedule is set to enabled, it runs immediately, default is false
        /// </summary>
        public bool RunImmediately { get; }

        /// <summary>
        /// The schedule value
        /// </summary>
        public string ScheduleValue
        {
            get {
                return string.Join("|", schedule.Select(x => x.Value));
            }
        }

        /// <summary>
        /// Running state
        /// </summary>
        public bool IsRunning
        {
            get {
                return flag > 0;
            }
        }

        /// <summary>
        /// Enable state
        /// </summary>
        public bool IsEnable
        {
            get {
                return status && !cancellationToken.IsCancellationRequested;
            }
        }

        bool status;

        /// <summary>
        /// Enable the executor
        /// </summary>
        /// <returns></returns>
        public bool Enable()
        {
            if (!status || flag > 0) {
                status = true;
                if (RunImmediately) {
                    Task.Factory.StartNew(RunExecute, cancellationToken);
                }
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Diable the executor
        /// </summary>
        /// <returns></returns>
        public bool Disable()
        {
            if (status) {
                status = false;
                return true;
            }
            else {
                return false;
            }
        }

        private bool Check(DateTime dateTime)
        {
            foreach (var s in schedule) {
                if (s.Check(dateTime)) {
                    return true;
                }
            }
            return false;
        }

        private void RunExecute()
        {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }
            Interlocked.Increment(ref flag);
            var serviceScope = services.CreateScope();
            var logger = services.GetService<ILogger>();
            try {
                var argtypes = type.GetConstructors()
                    .First()
                    .GetParameters()
                    .Select(x => serviceScope.ServiceProvider.GetService(x.ParameterType))
                    .ToArray();
                var instance = Activator.CreateInstance(type.AsType(), argtypes);


                var infos = method.GetParameters();
                var paramtypes = new object[infos.Length];
                for (int i = 0; i < infos.Length; i++) {
                    var info = infos[i];
                    if (info.ParameterType == typeof(CancellationToken)) {
                        paramtypes[i] = cancellationToken;
                    }
                    else {
                        paramtypes[i] = serviceScope.ServiceProvider.GetService(info.ParameterType);
                    }
                }
                var result = method.Invoke(instance, paramtypes);
                if (result is Task task) {
                    task.Wait();
                }
            }
            catch (Exception ex) {
                if (logger != null)
                    logger.LogError(ex.ToString());
            }
            finally {
                serviceScope.Dispose();
                Interlocked.Decrement(ref flag);
            }
        }

        /// <summary>
        /// Run the executor
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (!status) {
                return false;
            }
            var dateTime = DateTime.Now;
            if (!Check(dateTime)) {
                return false;
            }

            if (!AllowConcurrentExecution && flag > 0) {
                return false;
            }

            RunExecute();
            return true;
        }
    }
}
