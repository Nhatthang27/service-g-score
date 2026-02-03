using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Exceptions.Errors;

public static class CodeSessionErrors
{
    public static readonly Error NotFound = new Error(
        "CodeSession.NotFound",
        "The specified code session was not found.");
}
