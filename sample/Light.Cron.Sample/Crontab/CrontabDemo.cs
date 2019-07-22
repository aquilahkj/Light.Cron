using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Light.Cron.Sample.Crontab
{
    [CrontabJob]
    public class CrontabDemo
    {
        private readonly ILogger<CrontabDemo> logger;

        public CrontabDemo(ILogger<CrontabDemo> logger)
        {
            this.logger = logger;
        }

        [CrontabSchedule("test1", "*/3|*/5", RunImmediately = true)]
        public void Test1(CancellationToken token)
        {
            logger.LogInformation(DateTime.Now.ToString() + " test1 start");
            Thread.Sleep(5000);
            logger.LogInformation(DateTime.Now.ToString() + " test1 end");
        }


        [CrontabSchedule("test2", "*", RunImmediately = true)]
        public async Task Test2(CancellationToken token)
        {
            logger.LogInformation(DateTime.Now.ToString() + " test2 start");
            await Task.Delay(35000, token);
            logger.LogInformation(DateTime.Now.ToString() + " test2 end");
        }


        [CrontabSchedule("test3", "*", RunImmediately = true, AllowConcurrentExecution = true)]
        public async Task<int> Test3(CancellationToken token)
        {
            logger.LogInformation(DateTime.Now.ToString() + " test3 start");
            await Task.Delay(75000, token);
            logger.LogInformation(DateTime.Now.ToString() + " test3 end");
            return 1;
        }
    }
}
