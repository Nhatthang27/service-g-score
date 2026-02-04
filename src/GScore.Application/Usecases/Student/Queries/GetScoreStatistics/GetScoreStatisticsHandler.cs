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
        // Aggregate directly in database for better performance
        var statistics = await context.ExamScores
            .AsNoTracking()
            .Where(s => s.Score.HasValue)
            .GroupBy(s => s.Subject)
            .Select(g => new
            {
                Subject = g.Key,
                ExcellentCount = g.Count(s => s.Score >= 8.0m),
                GoodCount = g.Count(s => s.Score >= 6.0m && s.Score < 8.0m),
                AverageCount = g.Count(s => s.Score >= 4.0m && s.Score < 6.0m),
                BelowAverageCount = g.Count(s => s.Score < 4.0m),
                TotalCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        return statistics
            .Select(s => new SubjectStatisticsDto(
                GetSubjectName(s.Subject),
                s.ExcellentCount,
                s.GoodCount,
                s.AverageCount,
                s.BelowAverageCount,
                s.TotalCount))
            .OrderBy(s => s.Subject)
            .ToList();
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
