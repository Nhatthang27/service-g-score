using GScore.Application.Interfaces;
using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GScore.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<CodeSessionEntity> CodeSessions { get; set; }
    public DbSet<CodeExecutionEntity> CodeExecutions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
