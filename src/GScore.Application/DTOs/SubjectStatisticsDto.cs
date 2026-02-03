namespace GScore.Application.DTOs;

public record SubjectStatisticsDto(
    string Subject,
    int ExcellentCount,
    int GoodCount,
    int AverageCount,
    int BelowAverageCount,
    int TotalCount);
