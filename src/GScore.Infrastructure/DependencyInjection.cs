using GScore.Application.Interfaces;
using GScore.Infrastructure.Data;
using GScore.Infrastructure.Services;
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
        services.AddScoped<ICsvImportService, CsvImportService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Register PostgresSetting with IOptions pattern
        services.Configure<PostgresSetting>(configuration.GetSection(PostgresSetting.SectionName));

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
}
