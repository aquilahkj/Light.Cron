using System;
using System.Collections.Generic;
using Xunit;

namespace Light.Cron.Test
{
    public class UnitTestMinute
    {
        [Fact]
        public void Test_All1()
        {
            var values = new string[] { "*", "* *", "* * *", "* * * *", "* * * * *" };
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
        public void Test_All2()
        {
            var values = new string[] { "*/1", "*/1 */1", "*/1 */1 */1", "*/1 */1 */1 */1", "*/1 */1 */1 */1 */1" };
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
        public void Test_Pre2()
        {
            var values = new string[] { "*/2", "*/2 *", "*/2 * *", "*/2 * * *", "*/2 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute % 2 == 0)
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
            var values = new string[] { "*/5 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute % 5 == 0)
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
            var values = new string[] { "*/7 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute % 7 == 0)
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
            var values = new string[] { "10-40/7 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute >= 10 && date1.Minute <= 40 && (date1.Minute - 10) % 7 == 0)
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
            var values = new string[] { "59 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 59 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Minute))
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
            var values = new string[] { "20-59 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 59 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute >= 20 && date1.Minute <= 59)
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
            var values = new string[] { "11,23,59 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 11, 23, 59 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Minute))
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
            var values = new string[] { "11,23,36,47 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 11, 23, 36, 47 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Minute))
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
            var values = new string[] { "0-5,11,23,36,47 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 11, 23, 36, 47 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (list.Contains(date1.Minute) || (date1.Minute >= 0 && date1.Minute <= 5))
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
            var values = new string[] { "43-12 * * * *" };
            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if (date1.Minute <= 12 || date1.Minute >= 43)
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }

        [Fact]
        public void Test_Pre7_FTRE()
        {
            var values = new string[] { "43-12/7 * * * *" };

            foreach (var value in values) {
                var result = CrontabSchedule.TryParse(value, out CrontabSchedule schedule);
                Assert.True(result);
                var date = DateTime.Now.Date;
                List<int> list = new List<int>() { 43, 50, 57, 4, 11 };
                for (int i = 0; i < 100000; i++) {
                    var date1 = date.AddMinutes(i);
                    if ((date1.Minute <= 12 || date1.Minute >= 43) && list.Contains(date1.Minute))
                        Assert.True(schedule.Check(date1));
                    else {
                        Assert.False(schedule.Check(date1));
                    }
                }
            }
        }
    }
}
