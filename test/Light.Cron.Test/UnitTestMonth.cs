using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestMonth
    {

        [Fact]
        public void Test_Pre2()
        {
            var values = new string[] { "* * * */2", "* * * */2 *", "* * * */2 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 1000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Month - 1) % 2 == 0)
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
            var values = new string[] { "* * * */5 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Month - 1) % 5 == 0)
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
            var values = new string[] { "* * * */3 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Month - 1) % 3 == 0)
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
            var values = new string[] { "* * * 4-11/3 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Month >= 4 && date1.Month <= 11 && (date1.Month - 4) % 3 == 0)
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
            var values = new string[] { "* * * 1 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 1 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month))
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
            var values = new string[] { "* * * 12 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 12 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month))
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
            var values = new string[] { " * * * 2-10 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Month >= 2 && date1.Month <= 10)
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
            var values = new string[] { " * * * 2,5,12 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 2, 5, 12 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month))
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
            var values = new string[] { "* * * 3,6,11 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 3, 6, 11 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month))
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
            var values = new string[] { "* * * 1-5,7,11 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 7, 11 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month) || (date1.Month >= 1 && date1.Month <= 5))
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
            var values = new string[] { "* * * 8-2 *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Month <= 2 || date1.Month >= 8)
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
            var values = new string[] { "* * * 6-4/3 *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 6, 9, 12, 3 };
                for (int i = 0; i < 10000000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Month))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }


    }
}
