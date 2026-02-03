using Microsoft.OpenApi.Models;

namespace GScore.Presentation.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerConfig = configuration
            .GetSection(SwaggerSetting.SectionName)
            .Get<SwaggerSetting>() ?? new SwaggerSetting();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(swaggerConfig.Version, new OpenApiInfo
            {
                Title = swaggerConfig.Title,
                Version = swaggerConfig.Version,
                Description = swaggerConfig.Description
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
