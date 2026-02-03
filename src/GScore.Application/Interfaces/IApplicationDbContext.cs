using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<StudentEntity> Students { get; set; }
    DbSet<ExamScoreEntity> ExamScores { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
