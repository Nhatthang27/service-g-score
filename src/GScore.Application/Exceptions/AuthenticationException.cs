using GScore.Application.Exceptions.Errors;

namespace GScore.Application.Exceptions;

public class AuthenticationException : Exception
{
    public Error Error { get; }
    public AuthenticationException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}
