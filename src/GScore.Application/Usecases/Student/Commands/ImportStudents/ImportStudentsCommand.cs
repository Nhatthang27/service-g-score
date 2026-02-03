using GScore.Application.DTOs;
using MediatR;

namespace GScore.Application.Usecases.Student.Commands.ImportStudents;

public record ImportStudentsCommand : IRequest<ImportResultDto>
{
    public required Stream CsvStream { get; init; }
}
