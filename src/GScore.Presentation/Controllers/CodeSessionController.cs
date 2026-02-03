using GScore.Application.Usecases.CodeSession.Commands.CreateCodeSession;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GScore.Presentation.Controllers;

[Route("api/code-sessions")]
[ApiController]
public class CodeSessionController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateSessionResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateSessionCommand command)
    {
        return StatusCode(StatusCodes.Status201Created, await mediator.Send(command));
    }

    //[HttpPost("{sessionId}/run")]
    //[ProducesResponseType(typeof(RunCodeResponse), StatusCodes.Status202Accepted)]
    //public async Task<IActionResult> RunCode(
    //    Guid sessionId,
    //    [FromBody] RunCodeCommand command)
    //{
    //    command.SessionId = sessionId;
    //    var result = await _mediator.Send(command);
    //    return AcceptedAtAction(nameof(ExecutionsController.GetExecution),
    //        "Executions",
    //        new { executionId = result.ExecutionId },
    //        result);
    //}
}
