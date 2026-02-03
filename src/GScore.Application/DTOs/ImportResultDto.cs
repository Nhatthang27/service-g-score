namespace GScore.Application.DTOs;

public record ImportResultDto(
    int TotalRecords,
    int SuccessCount,
    int FailedCount,
    long DurationMs);
