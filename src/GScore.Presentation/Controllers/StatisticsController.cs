using GScore.Application.DTOs;
using GScore.Application.Usecases.Student.Queries.GetScoreStatistics;
using GScore.Presentation.Commons;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GScore.Presentation.Controllers;

[Route("api/statistics")]
[ApiController]
public class StatisticsController(IMediator mediator) : ControllerBase
{
    [HttpGet("score-distribution")]
    [ProducesResponseType(typeof(ApiResponse<List<SubjectStatisticsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScoreDistribution()
    {
        var query = new GetScoreStatisticsQuery();
        var result = await mediator.Send(query);

        return Ok(ApiResponse<List<SubjectStatisticsDto>>.SuccessResponse(result));
    }
}
