using GScore.Domain.Constants;

namespace GScore.Domain.Entities;

public class ExamScoreEntity : BaseEntity<Guid>
{
    public Guid StudentId { get; set; }
    public SubjectType Subject { get; set; }
    public decimal? Score { get; set; }
    public StudentEntity? Student { get; set; }
}
