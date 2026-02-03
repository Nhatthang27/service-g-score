using GScore.Application.DTOs;

namespace GScore.Application.Interfaces;

public interface ICsvImportService
{
    Task<ImportResultDto> ImportStudentsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken);
}
