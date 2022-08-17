using System.Threading.Tasks;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyCompanyName.MyProjectName;
using MyCompanyName.MyProjectName.EntityFrameworkCore;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace TestApp;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyProjectNameDomainModule),
    typeof(MyProjectNameEntityFrameworkCoreModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpCachingStackExchangeRedisModule)
)]
public class TestAppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
        
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "MyProjectName:";
        });
        
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("MyProjectName");
        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "MyProjectName-Protection-Keys");
        
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer
                .Connect(configuration["Redis:Configuration"]);
            return new 
                RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<TestAppModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        logger.LogInformation($"MySettingName => {configuration["MySettingName"]}");

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }
}
