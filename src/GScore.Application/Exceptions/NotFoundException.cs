using GScore.Application.Exceptions.Errors;

namespace GScore.Application.Exceptions;

public class NotFoundException : Exception
{
    public Error Error { get; }
    public NotFoundException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}
