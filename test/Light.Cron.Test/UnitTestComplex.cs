using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestComplex
    {
        [Fact]
        public void Test_All1()
        {
            var values = new string[] { "0 1 2-5 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day >= 2 && date1.Day <= 5 && date1.Hour == 1 && date1.Minute == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_All2()
        {
            var values = new string[] { "0 1 2-5 3,4,5,6 *", "0 1 2,3,4,5 3-6 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 1000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day >= 2 && date1.Day <= 5 && date1.Hour == 1 && date1.Minute == 0 && date1.Month >= 3 && date1.Month <= 6)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_All3()
        {
            var values = new string[] { "0 1 2-5 3,4,5,6 0,5", "0 1 2,3,4,5 3-6 0,5" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 1000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day >= 2 && date1.Day <= 5 && date1.Hour == 1 && date1.Minute == 0 && date1.Month >= 3 && date1.Month <= 6 && (date1.DayOfWeek == DayOfWeek.Sunday || date1.DayOfWeek == DayOfWeek.Friday))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }

        [Fact]
        public void Test_All4()
        {
            var values = new string[] { "*/5 */6 2-5 3,4,5,6 0,5", "0-59/5 0,6,12,18 2,3,4,5 3-6 0,5" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 1000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day >= 2 && date1.Day <= 5 && date1.Month >= 3 && date1.Month <= 6
                        && (date1.DayOfWeek == DayOfWeek.Sunday || date1.DayOfWeek == DayOfWeek.Friday)
                        && date1.Minute % 5 == 0
                        && date1.Hour % 6 == 0
                        )
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }
    }
}
