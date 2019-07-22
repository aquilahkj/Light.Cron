using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Light.Cron;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class CrontabExtensions
    {
        /// <summary>
        /// Add the CrontabJob service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
    /// <summary>
    /// 
    /// </summary>
    public static class CrontabExtensions
    {
        /// <summary>
        /// Use the CrontabJob
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCrontabJob(this IApplicationBuilder builder)
        {
            builder.ApplicationServices.GetRequiredService<CrontabService>();
            return builder;
        }
    }
}
