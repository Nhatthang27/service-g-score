using GScore.Application.Exceptions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Exceptions;

public class ValidationException : Exception
{
    public Error Error { get; }
    public ValidationException(Error error)
        : base(error.Message)
    {
        Error = error;
    }
}
