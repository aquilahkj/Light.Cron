using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestDay
    {

        [Fact]
        public void Test_Pre2()
        {
            var values = new string[] { "* * */2", "* * */2 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 1000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Day - 1) % 2 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre5()
        {
            var values = new string[] { "* * */5 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Day - 1) % 5 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre3()
        {
            var values = new string[] { "* * */3 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Day - 1) % 3 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre7_FT()
        {
            var values = new string[] { "* * 4-21/3 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day >= 4 && date1.Day <= 21 && (date1.Day - 4) % 3 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Start()
        {
            var values = new string[] { "* * 1 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                List<int> list = new List<int>() { 1 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Day))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }


        [Fact]
        public void Test_End()
        {
            var values = new string[] { "* * e * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.AddDays(1).Day == 1)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_End1()
        {
            var values = new string[] { "* * e1 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day == max.Day - 1)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_End12()
        {
            var values = new string[] { "* * e1,e2 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day == max.Day - 1 || date1.Day == max.Day - 2)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_FT_End()
        {
            var values = new string[] { " * * 15-e * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day >= 15 && date1.Day <= max.Day)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_FT_End1()
        {
            var values = new string[] { " * * 15-e1 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day >= 15 && date1.Day <= max.Day - 1) {
                        Assert.True(schedule.Check(date1), date1.ToString());
                    }
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_FT_End2()
        {
            var values = new string[] { " * * 15-e2 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day >= 15 && date1.Day <= max.Day - 2) {
                        Assert.True(schedule.Check(date1), date1.ToString());
                    }
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_FT_End12()
        {
            var values = new string[] { " * * e4-e2 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    var max = new DateTime(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
                    if (date1.Day >= max.Day - 4 && date1.Day <= max.Day - 2) {
                        Assert.True(schedule.Check(date1), date1.ToString());
                    }
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }


        [Fact]
        public void Test_Muti_End()
        {
            var values = new string[] { " * * 2,5,e * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                List<int> list = new List<int>() { 2, 5 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Day) || date1.AddDays(1).Day == 1)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }



        [Fact]
        public void Test_Muti()
        {
            var values = new string[] { "* * 3,6,11,21 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                List<int> list = new List<int>() { 3, 6, 11, 21 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Day))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Muti_FT()
        {
            var values = new string[] { "* * 1-5,7,11,21 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                List<int> list = new List<int>() { 7, 11, 21 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Day) || (date1.Day >= 1 && date1.Day <= 5))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1), date1.ToString());
                    }
                }
            }
        }


        [Fact]
        public void Test_FTRE()
        {
            var values = new string[] { "* * 18-2 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Day <= 2 || date1.Day >= 18)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre3_FTRE()
        {
            var values = new string[] { "* * 16-11/7 * *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = new DateTime(2000, 1, 1);
                List<int> list = new List<int>() { 16, 23, 30, 6 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Day))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }


    }
}
