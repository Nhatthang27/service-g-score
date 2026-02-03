using GScore.Application.Exceptions;
using GScore.Application.Exceptions.Errors;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using GScore.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Usecases.CodeSession.Commands.RunCode;

public class RunCodeHandler(
    IApplicationDbContext context,
    IBackgroundJobService backgroundJobService) : IRequestHandler<RunCodeCommand, RunCodeResponse>
{
    public async Task<RunCodeResponse> Handle(
        RunCodeCommand request,
        CancellationToken cancellationToken)
    {
        var session = await context.CodeSessions.FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken)
            ?? throw new NotFoundException(CodeSessionErrors.NotFound);

        var execution = new CodeExecutionEntity
        {
            SessionId = session.Id,
            Language = session.Language,
            SourceCode = session.SourceCode,
            Status = ExecutionStatus.Queued,
            QueuedAt = DateTime.UtcNow
        };

        context.CodeExecutions.Add(execution);
        await context.SaveChangesAsync(cancellationToken);

        // Queue for background execution
        backgroundJobService.Enqueue<ICodeExecutor>(executor =>
            executor.ExecuteAsync(execution.Language, execution.SourceCode, 10, 256));

        return new RunCodeResponse(execution.Id, execution.Status);
    }
}
