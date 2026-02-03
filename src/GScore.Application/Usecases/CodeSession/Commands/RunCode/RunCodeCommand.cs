using GScore.Domain.Constants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Usecases.CodeSession.Commands.RunCode;

public record RunCodeCommand : IRequest<RunCodeResponse>
{
    public required Guid SessionId { get; set; }
}

public record RunCodeResponse(Guid ExecutionId, ExecutionStatus Status);