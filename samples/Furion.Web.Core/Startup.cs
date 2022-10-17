﻿using Furion.Application;
using Furion.Schedule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Furion.Web.Core;

[AppStartup(700)]
public sealed class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConsoleFormatter();

        // 注册 JWT 授权
        services.AddJwt<AuthHandler>();

        services.AddCorsAccessor();

        services.AddControllersWithViews()
                // 配置多语言
                .AddAppLocalization()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.AddDateTimeTypeConverters();
                })
                .AddInjectWithUnifyResult();

        services.AddUnifyProvider<SpeciallyResultProvider>("specially");

        services.AddRemoteRequest();

        services.AddEventBus(options =>
        {
            options.AddFallbackPolicy<EventFallbackPolicy>();
        });

        // 添加实时通讯
        services.AddSignalR();

        services.AddFileLogging();

        services.AddDatabaseLogging<DatabaseLoggingWriter>();

        services.AddMonitorLogging();

        services.AddFromConvertBinding();

        services.AddSchedule(options =>
        {
            options.AddJob(JobSchedulerBuilder.Create(
                JobBuilder.Create<TestJob>().SetJobId("job1"),
                JobTriggerBuilder.Cron("* * * * *")
                , JobTriggerBuilder.Period(1000)
            ));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // 添加规范化结果状态码，需要在这里注册
        app.UseUnifyResultStatusCodes();

        app.UseHttpsRedirection();

        // 配置多语言，必须在 路由注册之前
        app.UseAppLocalization();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject();

        app.UseEndpoints(endpoints =>
        {
            // 批量注册集线器
            endpoints.MapHubs();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}