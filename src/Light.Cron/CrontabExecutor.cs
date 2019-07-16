using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Light.Cron
{
    class CrontabExecutor
    {
        public CrontabExecutor(TypeInfo typeInfo, MethodInfo method, CrontabSchedule[] schedule, bool skipWhileExecuting, bool runImmediately)
        {
            Method = method;
            Type = typeInfo;
            Identifier = Type.FullName + '.' + method.Name;
            Schedule = schedule;
            //HardCode = hardCode;
            SkipWhileExecuting = skipWhileExecuting;
            RunImmediately = runImmediately;
            Status = false;
            Enable = true;
        }


        public string Identifier { get; private set; }

        public TypeInfo Type { get; private set; }

        public MethodInfo Method { get; private set; }

        public CrontabSchedule[] Schedule { get; private set; }

        public bool SkipWhileExecuting { get; set; }

        public bool RunImmediately { get; set; }

        public bool Status { get; private set; }

        //public bool HardCode { get; private set; }

        public bool Enable { get; set; }

        private bool Check(DateTime dateTime)
        {
            foreach (var s in Schedule) {
                if (s.Check(dateTime)) {
                    return true;
                }
            }
            return false;
        }

        public bool Run(IServiceProvider services, DateTime dateTime, bool immediately)
        {
            if (SkipWhileExecuting && Status) {
                return false;
            }
            if (!Enable) {
                return false;
            }

            if (!Check(dateTime)) {
                if (!(immediately && RunImmediately))
                    return false;
            }
            Status = true;
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
                Status = false;
            }
            return true;
        }
    }
}
