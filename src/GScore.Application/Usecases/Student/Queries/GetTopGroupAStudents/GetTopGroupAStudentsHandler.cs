using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Usecases.Student.Queries.GetTopGroupAStudents;

public class GetTopGroupAStudentsHandler(IApplicationDbContext context)
    : IRequestHandler<GetTopGroupAStudentsQuery, List<TopStudentDto>>
{
    private const string TopGroupAQuery = """
        SELECT
            s.registration_number,
            MAX(CASE WHEN e.subject = 'TOAN' THEN e.score END) AS math_score,
            MAX(CASE WHEN e.subject = 'VATLI' THEN e.score END) AS physics_score,
            MAX(CASE WHEN e.subject = 'HOAHOC' THEN e.score END) AS chemistry_score,
            (MAX(CASE WHEN e.subject = 'TOAN' THEN e.score END) +
             MAX(CASE WHEN e.subject = 'VATLI' THEN e.score END) +
             MAX(CASE WHEN e.subject = 'HOAHOC' THEN e.score END)) AS total_score
        FROM students s
        INNER JOIN exam_scores e ON s.id = e.student_id
        WHERE e.subject IN ('TOAN', 'VATLI', 'HOAHOC')
          AND e.score IS NOT NULL
          AND e.deleted_at IS NULL
          AND s.deleted_at IS NULL
        GROUP BY s.id, s.registration_number
        HAVING COUNT(DISTINCT e.subject) = 3
        ORDER BY total_score DESC, s.registration_number
        LIMIT 10
        """;

    public async Task<List<TopStudentDto>> Handle(GetTopGroupAStudentsQuery request, CancellationToken cancellationToken)
    {
        var topStudents = await context.Database
            .SqlQueryRaw<TopGroupAResult>(TopGroupAQuery)
            .ToListAsync(cancellationToken);

        return topStudents
            .Select((s, index) => new TopStudentDto(
                index + 1,
                s.registration_number,
                s.math_score,
                s.physics_score,
                s.chemistry_score,
                s.total_score))
            .ToList();
    }
}

internal class TopGroupAResult
{
    public required string registration_number { get; init; }
    public decimal math_score { get; init; }
    public decimal physics_score { get; init; }
    public decimal chemistry_score { get; init; }
    public decimal total_score { get; init; }
}
