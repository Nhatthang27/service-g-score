namespace GScore.Application.DTOs;

public record TopStudentDto(
    int Rank,
    string RegistrationNumber,
    decimal MathScore,
    decimal PhysicsScore,
    decimal ChemistryScore,
    decimal TotalScore);
