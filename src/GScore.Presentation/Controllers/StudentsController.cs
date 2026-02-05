using GScore.Application.DTOs;
using GScore.Application.Exceptions.Errors;
using GScore.Application.Usecases.Student.Commands.ImportStudents;
using GScore.Application.Usecases.Student.Queries.GetStudentByRegistration;
using GScore.Application.Usecases.Student.Queries.GetTopGroupAStudents;
using GScore.Presentation.Commons;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GScore.Presentation.Controllers;

[Route("api/students")]
[ApiController]
public class StudentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("import")]
    [ProducesResponseType(typeof(ApiResponse<ImportResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<ImportResultDto>.ErrorResponse(
                new ApiError(CsvFileErrors.EmptyCsvFile.Code, CsvFileErrors.EmptyCsvFile.Message, null)));
        }

        using var stream = file.OpenReadStream();
        var command = new ImportStudentsCommand { CsvStream = stream };
        var result = await mediator.Send(command);

        return Ok(ApiResponse<ImportResultDto>.SuccessResponse(result));
    }

    [HttpGet("{registrationNumber}")]
    [ProducesResponseType(typeof(ApiResponse<StudentScoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(string registrationNumber)
    {
        var query = new GetStudentByRegistrationQuery { RegistrationNumber = registrationNumber };
        var result = await mediator.Send(query);

        return Ok(ApiResponse<StudentScoreDto>.SuccessResponse(result));
    }

    [HttpGet("top-group-a")]
    [ProducesResponseType(typeof(ApiResponse<List<TopStudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopGroupA()
    {
        var query = new GetTopGroupAStudentsQuery();
        var result = await mediator.Send(query);

        return Ok(ApiResponse<List<TopStudentDto>>.SuccessResponse(result));
    }
}
