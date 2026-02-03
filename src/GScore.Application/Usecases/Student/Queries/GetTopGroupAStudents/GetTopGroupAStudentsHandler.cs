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
        var studentsWithGroupA = await context.Students
            .AsNoTracking()
            .Include(s => s.Scores)
            .Where(s => s.Scores.Any(sc => sc.Subject == SubjectType.Toan && sc.Score.HasValue) &&
                        s.Scores.Any(sc => sc.Subject == SubjectType.VatLi && sc.Score.HasValue) &&
                        s.Scores.Any(sc => sc.Subject == SubjectType.HoaHoc && sc.Score.HasValue))
            .Select(s => new
            {
                s.RegistrationNumber,
                MathScore = s.Scores.First(sc => sc.Subject == SubjectType.Toan).Score!.Value,
                PhysicsScore = s.Scores.First(sc => sc.Subject == SubjectType.VatLi).Score!.Value,
                ChemistryScore = s.Scores.First(sc => sc.Subject == SubjectType.HoaHoc).Score!.Value
            })
            .ToListAsync(cancellationToken);

        var topStudents = studentsWithGroupA
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
            .Select((s, index) => new TopStudentDto(
                index + 1,
                s.RegistrationNumber,
                s.MathScore,
                s.PhysicsScore,
                s.ChemistryScore,
                s.TotalScore))
            .ToList();

        return topStudents;
    }
}
