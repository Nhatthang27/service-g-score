namespace GScore.Domain.Entities;

public class StudentEntity : BaseEntity<Guid>
{
    public required string RegistrationNumber { get; set; }
    public string? ForeignLanguageCode { get; set; }
    public ICollection<ExamScoreEntity> Scores { get; set; } = new List<ExamScoreEntity>();
}
