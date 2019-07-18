using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Light.Cron
{
    class CrontabExecutor
    {
        readonly IServiceProvider services;

        public CrontabExecutor(string name, IServiceProvider services, TypeInfo typeInfo, MethodInfo method, CrontabSchedule[] schedule, bool allowConcurrentExecution, bool runImmediately)
        {
            this.services = services;
            Name = name;
            Method = method;
            Type = typeInfo;
            Schedule = schedule;
            AllowConcurrentExecution = allowConcurrentExecution;
            RunImmediately = runImmediately;
            //Enable = true;
        }

        int flag;

        public string Name { get; }

        public TypeInfo Type { get; }

        public MethodInfo Method { get; }

        public CrontabSchedule[] Schedule { get; }

        public bool AllowConcurrentExecution { get; }

        public bool RunImmediately { get; }

        public bool Running
        {
            get {
                return flag > 0;
            }
        }

        bool status = false;

        public bool Enable()
        {
            if (!status || flag > 0) {
                status = true;
                if (RunImmediately) {
                    RunExecute();
                }
                return true;
            }
            else {
                return false;
            }
        }

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
            foreach (var s in Schedule) {
                if (s.Check(dateTime)) {
                    return true;
                }
            }
            return false;
        }

        private void RunExecute()
        {
            Interlocked.Increment(ref flag);
            var serviceScope = services.CreateScope();
            var logger = services.GetService<ILogger>();
            try {
                var argtypes = Type.GetConstructors()
                    .First()
                    .GetParameters()
                    .Select(x => serviceScope.ServiceProvider.GetService(x.ParameterType))
                    .ToArray();
                var instance = Activator.CreateInstance(Type.AsType(), argtypes);

                var paramtypes = Method.GetParameters().Select(x => serviceScope.ServiceProvider.GetService(x.ParameterType)).ToArray();
                Method.Invoke(instance, paramtypes);
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
