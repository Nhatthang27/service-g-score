using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using MediatR;

namespace GScore.Application.Usecases.Student.Commands.ImportStudents;

public class ImportStudentsHandler(ICsvImportService csvImportService) : IRequestHandler<ImportStudentsCommand, ImportResultDto>
{
    public async Task<ImportResultDto> Handle(ImportStudentsCommand request, CancellationToken cancellationToken)
    {
        return await csvImportService.ImportStudentsFromCsvAsync(request.CsvStream, cancellationToken);
    }
}
