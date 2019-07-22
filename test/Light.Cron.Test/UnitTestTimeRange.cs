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

        [Fact]
        public void Test_TimeRange_RE()
        {
            var values = new string[] { "17:30-05:30" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var hour = date1.Hour;
                    var minute = date1.Minute;

                    if ((hour > 17 || (hour == 17 && minute >= 30))||
                       (hour < 5 || (hour == 5 && minute <= 30)))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_TimeRange_RE_Week()
        {
            var values = new string[] { "17:30-05:30 * * 1-5" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var hour = date1.Hour;
                    var minute = date1.Minute;
                    var week = (int)date1.DayOfWeek;
                    if (((hour > 17 || (hour == 17 && minute >= 30)) && week >= 1 && week <= 5) ||
                       ((hour < 5 || (hour == 5 && minute <= 30)) && week >= 2 && week <= 6)) {
                        Assert.True(schedule.Check(date1));
                    }
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }

        [Fact]
        public void Test_TimeRange_RE_Day()
        {
            var values = new string[] { "17:30-05:30 1-5 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var hour = date1.Hour;
                    var minute = date1.Minute;
                    var day = date1.Day;
                    if (((hour > 17 || (hour == 17 && minute >= 30)) && day >= 1 && day <= 5) ||
                       ((hour < 5 || (hour == 5 && minute <= 30)) && day >= 2 && day <= 6)) {
                        Assert.True(schedule.Check(date1));
                    }
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }

        [Fact]
        public void Test_TimeRange_Pre5_RE()
        {
            var values = new string[] { "23:30-00:30/7" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var list = new List<DateTime>() {
                        date1.Date.AddHours(23).AddMinutes(30),
                        date1.Date.AddHours(23).AddMinutes(37),
                        date1.Date.AddHours(23).AddMinutes(44),
                        date1.Date.AddHours(23).AddMinutes(51),
                        date1.Date.AddHours(23).AddMinutes(58),
                        date1.Date.AddMinutes(5),
                        date1.Date.AddMinutes(12),
                        date1.Date.AddMinutes(19),
                        date1.Date.AddMinutes(26)
                    };

                    if (list.Contains(date1))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }
    }
}
