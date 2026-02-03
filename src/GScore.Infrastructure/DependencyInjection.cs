using Hangfire;
using Hangfire.Redis.StackExchange;
using GScore.Application.Interfaces;
using GScore.Infrastructure.BackgroundJobs;
using GScore.Infrastructure.Data;
using GScore.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GScore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddBackgroundJobs(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresSetting = configuration
            .GetSection(PostgresSetting.SectionName)
            .Get<PostgresSetting>()
            ?? throw new InvalidOperationException($"Configuration section '{PostgresSetting.SectionName}' is missing.");

        if (string.IsNullOrWhiteSpace(postgresSetting.ConnectionString))
            throw new InvalidOperationException($"'{nameof(PostgresSetting.ConnectionString)}' is required in '{PostgresSetting.SectionName}' configuration.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(postgresSetting.ConnectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration
            .GetSection(RedisSetting.SectionName)
            .Get<RedisSetting>()
            ?? throw new InvalidOperationException($"Configuration section '{RedisSetting.SectionName}' is missing.");

        if (string.IsNullOrWhiteSpace(redisSetting.ConnectionString))
            throw new InvalidOperationException($"'{nameof(RedisSetting.ConnectionString)}' is required in '{RedisSetting.SectionName}' configuration.");

        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redisSetting.ConnectionString, new RedisStorageOptions
                {
                    Prefix = $"{redisSetting.InstanceName}:"
                });
        });

        services.AddHangfireServer();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();

        return services;
    }
}
