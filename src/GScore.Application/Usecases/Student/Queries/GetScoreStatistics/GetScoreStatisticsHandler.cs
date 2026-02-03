using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Usecases.Student.Queries.GetScoreStatistics;

public class GetScoreStatisticsHandler(IApplicationDbContext context)
    : IRequestHandler<GetScoreStatisticsQuery, List<SubjectStatisticsDto>>
{
    public async Task<List<SubjectStatisticsDto>> Handle(GetScoreStatisticsQuery request, CancellationToken cancellationToken)
    {
        var scores = await context.ExamScores
            .AsNoTracking()
            .Where(s => s.Score.HasValue)
            .Select(s => new { s.Subject, s.Score })
            .ToListAsync(cancellationToken);

        var statistics = scores
            .GroupBy(s => s.Subject)
            .Select(g => new SubjectStatisticsDto(
                GetSubjectName(g.Key),
                g.Count(s => s.Score >= 8.0m),
                g.Count(s => s.Score >= 6.0m && s.Score < 8.0m),
                g.Count(s => s.Score >= 4.0m && s.Score < 6.0m),
                g.Count(s => s.Score < 4.0m),
                g.Count()))
            .OrderBy(s => s.Subject)
            .ToList();

        return statistics;
    }

    private static string GetSubjectName(SubjectType subject) => subject switch
    {
        SubjectType.Toan => "toan",
        SubjectType.NguVan => "ngu_van",
        SubjectType.NgoaiNgu => "ngoai_ngu",
        SubjectType.VatLi => "vat_li",
        SubjectType.HoaHoc => "hoa_hoc",
        SubjectType.SinhHoc => "sinh_hoc",
        SubjectType.LichSu => "lich_su",
        SubjectType.DiaLi => "dia_li",
        SubjectType.GDCD => "gdcd",
        _ => subject.ToString().ToLower()
    };
}
