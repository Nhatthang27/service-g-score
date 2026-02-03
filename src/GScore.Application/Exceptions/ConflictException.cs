using GScore.Application.Exceptions.Errors;

namespace GScore.Application.Exceptions;

public class ConflictException : Exception
{
    public Error Error { get; }
    public ConflictException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}
