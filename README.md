# Light.Cron

[![dotnet](https://img.shields.io/badge/dotnet%20standard-2.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/aquilahkj/Light.Data2/blob/master/LICENSE)

`Light.Cron`是一个使用简单的基于`dotnet standard 2.0`的定时任务组件, 通过执行对象和对执行方法的`Attribute`进行定时任务配置

nuget 安装
```bash
PM> Install-Package Light.Cron
```

## 使用配置

```csharp
[CrontabJob]
public class CrontabObject
{
    [CrontabSchedule("crontab1", "* * * * *")]
    public void DoSomeThing()
    {
        // Todo
    }
}
```

执行对象需要标记`CrontabJobAttribute`, 执行方法需要标记`CrontabScheduleAttribute`

`CrontabScheduleAttribute`

| 属性 | 说明 |
|:------|:------|
| Name | 指定Crontab方法的唯一名称 |
| Schedule | 调度计划, 如`* * * * *` |
| AllowConcurrentExecution | 允许并发执行, 如上一次执行还没结束, 下一次执行时间已到, 允许同时执行 |
| RunImmediately | 启用时不判断`Schedule`是否符合, 马上执行 |
| AutoEnable | CrontabService启动时自动启用 |

系统会通过使用依赖注入方式生成一个`Singleton`的`CrontabService`对象进行定时任务调度.

通过使用`IServiceCollection.AddCrontabJob`依赖注入配置, 再使用`IApplicationBuilder.UseCrontabJob`启动`CrontabService`.

```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCrontabJob();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCrontabJob();
            app.UseMvc();
        }
    }
```

`CrontabService`默认会扫描`EntryAssembly`程序集中的有`CrontabJobAttribute`的类以及该类中有`CrontabScheduleAttribute`的方法, 如要扫描其他程序集, 可以在`AddCrontabJob`中可以使用`CrontabOptionsBuilder`进行配置

| 属性/方法 | 说明 |
|:------|:------|
| ScanAllAssembly | 扫描所有程序集 |
| SetAssembly | 扫描指定的程序集 |


## 调度命令格式

`Light.Cron`完全兼容Linux Crontab时间格式, 如下

```
*　　*　　*　　*　　*
分　时　日　月　周
```
第1列表示分钟0～59 
第2列表示小时0～23
第3列表示日期1～31 新增`e`标记作为月末最后一天
第4列表示月份1～12 
第5列标识号星期0～7（0和7表示星期天） 

```
每天早上6点 
0 6 * * *

每天10点至16点 
0 10-16 * * *

每隔两个小时 
0 */2 * * *

每月1日，15日和最后1日的早上5点
0 5 1,15,e * * 
```

### 跨时间段
当时间段格式出现结束时间小于开始时间, 则代表由开始时间到下一级时间的开始时间, 如
```
每月1号22点到2号4点之间的时间段
* 22-4 1 * * 
每周一至周五晚上22点到次日4点之间的时间段, 里面包含周六的0点-4点, 不包含周一的0点-4点
* 22-4 * * 1-5
```

### 月末日期
日期格式中增加新增`e`标记作为月末最后一天, 并且可以通过`e+数字`代表最后一天再往前倒数天数, 如当月共31天, 则e=31, e1=30, e2=29, e3=28. 当月共30天, 则e=30, e1=29, e2=28, e3=27. 
该语法同样支持范围
```
0 0 e3-e1 * * 
```

### 时间范围
`Light.Cron`可以通过新语法`HH:mm-HH:mm`将时分结合, 替换原有的时分设置, 如

```
每日9点30分至15点0分, 每隔一分钟执行一次
09:30-15:00 * * *
每日21点30分至次日的5点30分, 每隔5分钟执行一次
21:30-05:30/5 * * *
```

### 多组调度计划
`Light.Cron`可以通过`|`符号支持多组调度计划, 如
```
0点至11点, 每分钟执行一次, 12点至23点, 每5分钟执行一次
* 0-11 * * *|*/5 12-23 * * *
```

### 简略写法
如下一级及其以之后的时间范围都是`*`, 可以简略不写, 如
```
* * * * * = *
30 9 * * * = 30 9
09:30-15:00 * * * = 09:30-15:00
0 0 1 * * = 0 0 1
0 0 1 2-5 * = 0 0 1 2-5
```
