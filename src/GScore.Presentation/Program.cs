using Hangfire;
using GScore.Presentation.Configurations;
using GScore.Presentation.Middlewares;
using GScore.Application;
using GScore.Infrastructure;
using GScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddJsonOptions();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var defaultOrigin = "http://localhost:5173";
var corsOrigins = builder.Configuration["Cors:AllowedOrigins"];
var origins = string.IsNullOrEmpty(corsOrigins)
    ? new[] { defaultOrigin }
    : new[] { defaultOrigin }.Concat(corsOrigins.Split(",")).ToArray();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
