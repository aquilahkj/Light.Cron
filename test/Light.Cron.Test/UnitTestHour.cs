using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestHour
    {

        [Fact]
        public void Test_Pre2()
        {
            var values = new string[] { "* */2", "* */2 *", "* */2 * *", "* */2 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour % 2 == 0)
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
            var values = new string[] { "* */5 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour % 5 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre7()
        {
            var values = new string[] { "* */7 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour % 7 == 0)
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
            var values = new string[] { "* 10-22/7 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour >= 10 && date1.Hour <= 22 && (date1.Hour - 10) % 7 == 0)
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
            var values = new string[] { "* 23 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 23 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Hour))
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
            var values = new string[] { "* 10-23 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour >= 10 && date1.Hour <= 23)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Muti_End()
        {
            var values = new string[] { "* 5,11,23 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 5, 11, 23 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Hour))
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
            var values = new string[] { "* 3,6,11,22 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 3, 6, 11, 22 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Hour))
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
            var values = new string[] { "* 0-5,11,22 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 11, 22 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Hour) || (date1.Hour >= 0 && date1.Hour <= 5))
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
            var values = new string[] { "* 8-2 * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Hour <= 2 || date1.Hour >= 8)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_FTRE_Hour()
        {
            var values = new string[] { "* 8-2 1-5 * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var hour = date1.Hour;
                    var day = date1.Day;
                    if ((date1.Hour >= 8 && day >= 1 && day <= 5) || (date1.Hour <= 2 && day >= 2 && day <= 6))
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
            var values = new string[] { "* 13-5/3 * * *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 13, 16, 19, 22, 1, 4 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Hour <= 5 || date1.Hour >= 13) && list.Contains(date1.Hour))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre3_FTRE_Hour()
        {
            var values = new string[] { "* 13-5/3 1-5 * *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 13, 16, 19, 22, 1, 4 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    var hour = date1.Hour;
                    var day = date1.Day;
                    if (((date1.Hour >= 13 && day >= 1 && day <= 5) || (date1.Hour <= 5 && day >= 2 && day <= 6)) && list.Contains(hour))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre3_FTRE2()
        {
            var values = new string[] { "* 12-6/3 * * *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 12, 15, 18, 21, 0, 3, 6 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Hour <= 6 || date1.Hour >= 12) && list.Contains(date1.Hour))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }
    }
}
