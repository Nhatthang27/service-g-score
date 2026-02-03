using GScore.Application.DTOs;
using MediatR;

namespace GScore.Application.Usecases.Student.Queries.GetStudentByRegistration;

public record GetStudentByRegistrationQuery : IRequest<StudentScoreDto>
{
    public required string RegistrationNumber { get; init; }
}
