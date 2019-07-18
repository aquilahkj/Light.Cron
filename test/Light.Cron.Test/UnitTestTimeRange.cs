using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestTimeRange
    {
        [Fact]
        public void Test_All_TimeRange()
        {
            var values = new string[] { "00:00-23:59", "00:00-23:59 *", "00:00-23:59 * *", "00:00-23:59 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    Assert.True(schedule.Check(date1));
                }
            }
        }

        [Fact]
        public void Test_TimeRange()
        {
            var values = new string[] { "01:00-13:25 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var datef = date1.Date.AddHours(1);
                    var datee = date1.Date.AddHours(13).AddMinutes(25);
                    if (date1 >= datef && date1 <= datee)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_TimeRange_Muti()
        {
            var values = new string[] { "01:00-13:25,16:00-19:25,18:00-22:22 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var datef1 = date1.Date.AddHours(1);
                    var datee1 = date1.Date.AddHours(13).AddMinutes(25);

                    var datef2 = date1.Date.AddHours(16);
                    var datee2 = date1.Date.AddHours(22).AddMinutes(22);
                    if ((date1 >= datef1 && date1 <= datee1) || (date1 >= datef2 && date1 <= datee2))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_TimeRange_Muti_Pre7()
        {
            var values = new string[] { "01:00-13:25/7,16:00-19:25/7,18:00-22:22/7 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var datef1 = date1.Date.AddHours(1);
                    var datee1 = date1.Date.AddHours(13).AddMinutes(25);

                    var datef2 = date1.Date.AddHours(16);
                    var datee2 = date1.Date.AddHours(19).AddMinutes(25);

                    var datef3 = date1.Date.AddHours(18);
                    var datee3 = date1.Date.AddHours(22).AddMinutes(22);
                    if (date1 >= datef1 && date1 <= datee1 && Convert.ToInt32((date1 - datef1).TotalMinutes) % 7 == 0)
                        Assert.True(schedule.Check(date1));
                    else if (date1 >= datef2 && date1 <= datee2 && Convert.ToInt32((date1 - datef2).TotalMinutes) % 7 == 0)
                        Assert.True(schedule.Check(date1));
                    else if (date1 >= datef3 && date1 <= datee3 && Convert.ToInt32((date1 - datef3).TotalMinutes) % 7 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }
    }
}
