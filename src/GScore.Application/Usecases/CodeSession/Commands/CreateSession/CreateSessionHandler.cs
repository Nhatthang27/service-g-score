using GScore.Application.Interfaces;
using GScore.Application.Usecases.CodeSession.Commands.CreateCodeSession;
using GScore.Domain.Constants;
using GScore.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Usecases.CodeSession.Commands.CreateSession;

public class CreateSessionHandler(IApplicationDbContext context) : IRequestHandler<CreateSessionCommand, CreateSessionResponse>
{
    public async Task<CreateSessionResponse> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new CodeSessionEntity
        {
            Language = request.Language,
            SourceCode = request.TemplateCode,
            Status = SessionStatus.Active,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        context.CodeSessions.Add(session);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateSessionResponse(session.Id, session.Status);
    }
}
