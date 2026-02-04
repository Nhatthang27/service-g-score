using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GScore.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<StudentEntity> Students { get; set; }
    DbSet<ExamScoreEntity> ExamScores { get; set; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
