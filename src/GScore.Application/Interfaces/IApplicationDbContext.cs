using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<CodeSessionEntity> CodeSessions { get; set; }
    DbSet<CodeExecutionEntity> CodeExecutions { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
