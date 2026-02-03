using FluentValidation;
using GScore.Application.Constants;
using GScore.Application.Usecases.CodeSession.Commands.CreateCodeSession;

namespace GScore.Application.Usecases.CodeSession.Commands.CreateSession;

public class CreateSessionValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty()
            .Must(Languages.IsSupported)
            .WithMessage("Unsupported language");
    }
}
