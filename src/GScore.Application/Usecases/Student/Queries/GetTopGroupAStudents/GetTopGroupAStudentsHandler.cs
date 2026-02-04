using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Usecases.Student.Queries.GetTopGroupAStudents;

public class GetTopGroupAStudentsHandler(IApplicationDbContext context)
    : IRequestHandler<GetTopGroupAStudentsQuery, List<TopStudentDto>>
{
    public async Task<List<TopStudentDto>> Handle(GetTopGroupAStudentsQuery request, CancellationToken cancellationToken)
    {
        // Query directly from ExamScores, group by student, compute in database
        var topStudents = await context.ExamScores
            .AsNoTracking()
            .Where(e => e.Score.HasValue &&
                        (e.Subject == SubjectType.Toan ||
                         e.Subject == SubjectType.VatLi ||
                         e.Subject == SubjectType.HoaHoc))
            .GroupBy(e => new { e.StudentId, e.Student!.RegistrationNumber })
            .Where(g => g.Count() == 3) // Must have all 3 subjects
            .Select(g => new
            {
                g.Key.RegistrationNumber,
                MathScore = g.Where(e => e.Subject == SubjectType.Toan).Select(e => e.Score!.Value).FirstOrDefault(),
                PhysicsScore = g.Where(e => e.Subject == SubjectType.VatLi).Select(e => e.Score!.Value).FirstOrDefault(),
                ChemistryScore = g.Where(e => e.Subject == SubjectType.HoaHoc).Select(e => e.Score!.Value).FirstOrDefault()
            })
            .Select(s => new
            {
                s.RegistrationNumber,
                s.MathScore,
                s.PhysicsScore,
                s.ChemistryScore,
                TotalScore = s.MathScore + s.PhysicsScore + s.ChemistryScore
            })
            .OrderByDescending(s => s.TotalScore)
            .ThenBy(s => s.RegistrationNumber)
            .Take(10)
            .ToListAsync(cancellationToken);

        return topStudents
            .Select((s, index) => new TopStudentDto(
                index + 1,
                s.RegistrationNumber,
                s.MathScore,
                s.PhysicsScore,
                s.ChemistryScore,
                s.TotalScore))
            .ToList();
    }
}
