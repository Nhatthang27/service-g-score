namespace GScore.Application.DTOs;

public record StudentScoreDto(
    string RegistrationNumber,
    string? ForeignLanguageCode,
    Dictionary<string, decimal?> Scores);
