using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Light.Cron;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TimedJobExtensions
    {
        public static IServiceCollection AddCrontabJob(this IServiceCollection services, Action<CrontabOptionsBuilder> options = null)
        {
            var builder = new CrontabOptionsBuilder();
            options?.Invoke(builder);
            builder.BuildServices(services);
            return services;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class TimedJobExtensions
    {
        public static IApplicationBuilder UseCrontabJob(this IApplicationBuilder self)
        {
            self.ApplicationServices.GetRequiredService<CrontabService>();
            return self;
        }
    }
}
