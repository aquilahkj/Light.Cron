using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestWeek
    {

        [Fact]
        public void Test_Pre2()
        {
            var values = new string[] { "* * * * */2" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek % 2 == 0)
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
            var values = new string[] { "* * * * */5" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek % 5 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre6()
        {
            var values = new string[] { "* * * * */6" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek % 6 == 0)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre2_FT()
        {
            var values = new string[] { "* * * * 1-5/2" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek >= 1 && (int)date1.DayOfWeek <= 5 && ((int)date1.DayOfWeek - 1) % 2 == 0)
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
            var values = new string[] { "* * * * 7", "* * * * 0" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 0 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains((int)date1.DayOfWeek))
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
            var values = new string[] { "* * * * 3-7", "* * * * 3-0" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek >= 3 || (int)date1.DayOfWeek == 0)
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
            var values = new string[] { "* * * * 2,3,5,7", "* * * * 2,3,5,0" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 2, 3, 5, 0 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains((int)date1.DayOfWeek))
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
            var values = new string[] { "* * * * 1,3,6" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 1, 3, 6 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains((int)date1.DayOfWeek))
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
            var values = new string[] { "* * * * 0-2,4,5", "* * * * 7-2,4,5" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 4, 5 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains((int)date1.DayOfWeek) || ((int)date1.DayOfWeek >= 0 && (int)date1.DayOfWeek <= 2))
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
            var values = new string[] { "* * * * 4-2" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((int)date1.DayOfWeek <= 2 || (int)date1.DayOfWeek >= 4)
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
            var values = new string[] { "* * * * 5-3/2" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 5, 0, 2 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains((int)date1.DayOfWeek))
                        Assert.True(schedule.Check(date1), date1.ToString());
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }
    }
}
