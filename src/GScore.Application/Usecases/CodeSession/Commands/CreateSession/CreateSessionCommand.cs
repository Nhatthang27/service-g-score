using GScore.Domain.Constants;
using MediatR;

namespace GScore.Application.Usecases.CodeSession.Commands.CreateCodeSession;

public record CreateSessionCommand : IRequest<CreateSessionResponse>
{
    public string TemplateCode { get; init; } = string.Empty;
    public required string Language { get; init; }
}

public record CreateSessionResponse(Guid SessionId, SessionStatus Status);
